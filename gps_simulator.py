import requests
import time
import json
import random
import threading

# Configuration
# Đảm bảo URL này khớp với app của bạn (thường là http://localhost:5045 hoặc 5000...)
API_URL = "http://localhost:5045/api/Gps/Update"
VEHICLE_IDS = [1, 2, 3]  # Danh sách ID xe cần giả lập (phải có trong DB)
INTERVAL = 5             # Giây (Cập nhật 2s một lần theo yêu cầu)

class VehicleSimulator(threading.Thread):
    def __init__(self, vehicle_id):
        threading.Thread.__init__(self)
        self.vehicle_id = vehicle_id
        # Tọa độ bắt đầu ngẫu nhiên quanh khu vực TP.HCM để không bị trùng nhau
        self.lat = 10.762622 + random.uniform(-0.02, 0.02)
        self.lng = 106.660172 + random.uniform(-0.02, 0.02)
        self.running = True

    def run(self):
        print(f"🚀 Khởi chạy simulator cho Xe ID: {self.vehicle_id}")
        while self.running:
            # Di chuyển ngẫu nhiên một khoảng nhỏ
            # Tăng/giảm tọa độ để tạo hiệu ứng xe đang chạy
            self.lat += random.uniform(-0.0003, 0.0003)
            self.lng += random.uniform(-0.0003, 0.0003)
            
            # Vận tốc ngẫu nhiên, đôi khi vượt quá 80 để test cảnh báo
            speed = random.uniform(20, 95)

            # Chuẩn bị dữ liệu JSON
            data = {
                "VehicleID": self.vehicle_id,
                "Latitude": self.lat,
                "Longitude": self.lng,
                "Speed": speed
            }

            try:
                # Gửi request POST đến API
                response = requests.post(API_URL, json=data, timeout=5)
                if response.status_code == 200:
                    status = "THÀNH CÔNG"
                else:
                    status = f"LỖI {response.status_code}"
                
                print(f"[{time.strftime('%H:%M:%S')}] Xe {self.vehicle_id}: {status} | Tọa độ: {self.lat:.5f}, {self.lng:.5f} | Tốc độ: {speed:.1f} km/h")
                
                if speed > 80:
                    print(f"   ⚠️  Xe {self.vehicle_id} đang chạy quá tốc độ!")
                    
            except Exception as e:
                print(f"[{time.strftime('%H:%M:%S')}] Xe {self.vehicle_id}: MẤT KẾT NỐI - {str(e)}")

            # Đợi đến chu kỳ tiếp theo
            time.sleep(INTERVAL)

    def stop(self):
        self.running = False

if __name__ == "__main__":
    print(f"==========================================")
    print(f"   GIẢ LẬP THIẾT BỊ GPS ĐA PHƯƠNG TIỆN    ")
    print(f"==========================================")
    print(f"API đích: {API_URL}")
    print(f"Đang giả lập cho các xe IDs: {VEHICLE_IDS}")
    print(f"Tần suất: {INTERVAL} giây/lần")
    print(f"Nhấn Ctrl+C để dừng toàn bộ\n")

    simulators = []

    try:
        for vid in VEHICLE_IDS:
            sim = VehicleSimulator(vid)
            sim.start()
            simulators.append(sim)
            # Khởi hành lệch nhau một chút để logging nhìn đẹp hơn
            time.sleep(0.5)

        # Giữ luồng chính sống
        while True:
            time.sleep(1)

    except KeyboardInterrupt:
        print("\n đang dừng tất cả simulator...")
        for sim in simulators:
            sim.stop()
        for sim in simulators:
            sim.join()
        print("Đã dừng toàn bộ simulator.")
