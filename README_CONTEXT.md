# Project Context: GPS Vehicle Management System

This document provides a high-level overview of the project for future AI agents and developers.

## 🚀 Tech Stack

- **Backend**: ASP.NET Core 8.0 (MVC & Web API)
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **GIS**: NetTopologySuite (Handling spatial data/Geography types)
- **Real-time**: SignalR (Live dashboard updates)
- **Frontend**: 
  - **CSS Framework**: AdminLTE 3 (Bootstrap 4 based)
  - **Maps**: Leaflet.js
  - **Icons**: FontAwesome 5
- **Simulation**: Python 3 (using `requests` and `threading`)

## 📊 Database Schema Summary

| Table | Description | Key Relationships |
| :--- | :--- | :--- |
| **Vehicle** | Stores vehicle information and latest GPS coordinates. | Linked to `Device` via `DeviceId` (Nullable). |
| **Device** | Represents the GPS tracking hardware. | Linked to `Vehicle` (1:1). |
| **GPSHistory** | Stores history of coordinates and speeds. | Linked to `Device` via `DeviceId` (N:1). |
| **Alert** | Stores system alerts (e.g., Overspeed). | Linked to `Vehicle` via `VehicleId`. |
| **Customer** | Stores client information. | Linked to `Rental`. |
| **Rental** | Manages vehicle-customer assignments. | Linked to `Vehicle` and `Customer`. |

> [!NOTE]
> Spatial data (Latitude/Longitude) is stored using the `Point` type from NetTopologySuite (SRID 4326).

## 🔌 Key API Endpoints

- **`POST /api/Gps/Update`**: 
  - **Input**: `GpsDataDto` { `VehicleID`, `Latitude`, `Longitude`, `Speed` }
  - **Action**: Saves to `GPSHistory`, updates `Vehicle.LastLatitude/LastLongitude`, checks overspeed (>80km/h), and triggers SignalR `UpdateStats`.
- **`GET /api/Gps/{id}`**: 
  - **Output**: Latest location and speed for a specific vehicle.

## 📁 Project Structure

- **Controllers**:
  - `HomeController`: Manages the Dashboard overview.
  - `VehiclesController`: Vehicle CRUD and Real-time Tracking view (`/Vehicles/Tracking/{id}`).
  - `HistoryController`: Historical data search and playback view (`/History/Index`).
  - `GpsController`: API for receiving device data.
- **Views**:
  - `Home/Index.cshtml`: Main dashboard with SignalR-powered stats and Leaflet map.
  - `Vehicles/Tracking.cshtml`: Individual vehicle live tracking.
  - `History/Index.cshtml`: History playback with range slider.
- **Hubs**:
  - `DashboardHub`: SignalR hub for broadcasting live stats.

## 🤖 Simulation Details

- **File**: `[gps_simulator.py](file:///home/duy/DACS/gps_simulator.py)`
- **Mechanism**: Multi-threaded script where each thread simulates one vehicle.
- **Vehicle IDs**: Defaulting to `[1, 2, 3]` (must exist in DB).
- **Movement**: Logic simulates random movement around Ho Chi Minh City coordinates.
- **Command**: `python3 gps_simulator.py` (ensure `requests` is installed).

## 💡 Important Notes
- The system uses **SignalR** to push updates to the dashboard without refreshing.
- **AdminLTE** sidebar handles navigation between Dashboard, Vehicles, and History.
- GPS coordinates are processed as `Coordinate(Longitude, Latitude)` in NTS.
