import requests
import time
import json
import random
import threading

# CONFIGURATION
API_URL = "http://localhost:5045/api/Gps/Update"
INTERVAL = 3    # Khoảng cách giữa các lần gửi (giây)

# Danh sách cấu hình xe giả lập
VEHICLES_CONFIG = [
    {
        "id": 1,
        "name": "Xe 01 (Quận 1 -> Quận 7)",
        "start": "106.6983,10.7715", # Long,Lat
        "end": "106.7011,10.7291"
    },
    {
        "id": 3,
        "name": "Xe 03 (Nhà Duy -> HUTECH)",
        "start": "106.7900,10.8500", # Nhà (Khu Quận 9/Thủ Đức)
        "end": "106.7119,10.8020"    # Đại học HUTECH (Cơ sở Điện Biên Phủ)
    }
]

def get_route(config):
    print(f"📡 Đang truy vấn lộ trình cho {config['name']}...")
    url = f"http://router.project-osrm.org/route/v1/driving/{config['start']};{config['end']}?overview=full&geometries=geojson"
    try:
        response = requests.get(url)
        if response.status_code == 200:
            data = response.json()
            coordinates = data['routes'][0]['geometry']['coordinates']
            print(f"✅ {config['name']}: Đã tìm thấy lộ trình với {len(coordinates)} điểm.")
            return coordinates
        else:
            print(f"❌ {config['name']}: Lỗi OSRM {response.status_code}")
            return None
    except Exception as e:
        print(f"❌ {config['name']}: Lỗi kết nối OSRM: {str(e)}")
        return None

def simulate_vehicle(config):
    route = get_route(config)
    if not route: return

    vehicle_id = config['id']
    print(f"🚀 Bắt đầu giả lập {config['name']} (ID: {vehicle_id})...")
    
    for i, point in enumerate(route):
        lng, lat = point
        speed = random.uniform(30, 60)
        
        payload = {
            "VehicleID": vehicle_id,
            "Latitude": lat,
            "Longitude": lng,
            "Speed": speed
        }

        try:
            res = requests.post(API_URL, json=payload, timeout=5)
            status = "OK" if res.status_code == 200 else f"ERR {res.status_code}"
            print(f"[{i+1}/{len(route)}] {config['name']}: {status} | {lat:.5f}, {lng:.5f} | {speed:.1f} km/h")
        except Exception as e:
            print(f"❌ {config['name']}: Lỗi gửi dữ vote: {str(e)}")

        time.sleep(INTERVAL)

    print(f"🏁 {config['name']} đã hoàn thành lộ trình!")

def run_simulation():
    threads = []
    for config in VEHICLES_CONFIG:
        t = threading.Thread(target=simulate_vehicle, args=(config,))
        t.start()
        threads.append(t)
        # Delay nhẹ giữa các xe để tránh spam API dính chùm
        time.sleep(1)

    for t in threads:
        t.join()

if __name__ == "__main__":
    run_simulation()
