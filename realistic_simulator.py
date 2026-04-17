import requests
import time
import json
import random

# CONFIGURATION
API_URL = "http://localhost:5045/api/Gps/Update"
VEHICLE_ID = 1  # Đảm bảo ID này tồn tại trong Database
INTERVAL = 3    # Khoảng cách giữa các lần gửi (giây)

# Tọa độ Quận 1 (Chợ Bến Thành) và Quận 7 (Lotte Mart)
START_COORD = "106.6983,10.7715" # Long,Lat cho OSRM
END_COORD = "106.7011,10.7291"   # Long,Lat cho OSRM

def get_route():
    print(f"📡 Đang truy vấn lộ trình từ OSRM (Quận 1 -> Quận 7)...")
    url = f"http://router.project-osrm.org/route/v1/driving/{START_COORD};{END_COORD}?overview=full&geometries=geojson"
    try:
        response = requests.get(url)
        if response.status_code == 200:
            data = response.json()
            # Lấy danh sách tọa độ từ GeoJSON
            coordinates = data['routes'][0]['geometry']['coordinates']
            print(f"✅ Đã tìm thấy lộ trình với {len(coordinates)} điểm tọa độ.")
            return coordinates
        else:
            print(f"❌ Lỗi truy vấn OSRM: {response.status_code}")
            return None
    except Exception as e:
        print(f"❌ Lỗi kết nối OSRM: {str(e)}")
        return None

def simulate():
    route = get_route()
    if not route:
        return

    print(f"🚀 Bắt đầu giả lập Xe {VEHICLE_ID}...")
    
    # Duyệt qua từng cặp tọa độ trong lộ trình
    for i, point in enumerate(route):
        # OSRM trả về [longitude, latitude]
        lng, lat = point
        
        # Tạo tốc độ ngẫu nhiên từ 30 đến 60 km/h
        speed = random.uniform(30, 60)
        
        # Gửi dữ liệu lên API
        payload = {
            "VehicleID": VEHICLE_ID,
            "Latitude": lat,
            "Longitude": lng,
            "Speed": speed
        }

        try:
            res = requests.post(API_URL, json=payload, timeout=5)
            status = "OK" if res.status_code == 200 else f"ERROR {res.status_code}"
            print(f"[{i+1}/{len(route)}] Xe {VEHICLE_ID}: {status} | Tọa độ: {lat:.6f}, {lng:.6f} | Tốc độ: {speed:.1f} km/h")
        except Exception as e:
            print(f"❌ Lỗi gửi dữ liệu: {str(e)}")

        # Đợi 1 chút để mô phỏng thời gian di chuyển
        time.sleep(INTERVAL)

    print("🏁 Đã hoàn thành lộ trình!")

if __name__ == "__main__":
    simulate()
