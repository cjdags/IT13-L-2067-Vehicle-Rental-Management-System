-- =============================================
-- Vehicle Rental Management System Database Schema
-- Compatible with MySQL and SQLyog
-- Based on ERD.puml specification
-- =============================================

-- Drop database if exists (for fresh start)
DROP DATABASE IF EXISTS vehicle_rental_db;
CREATE DATABASE vehicle_rental_db;
USE vehicle_rental_db;

-- =============================================
-- Table: Users
-- =============================================
CREATE TABLE Users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    phone VARCHAR(20),
    role ENUM('Admin', 'Rental Agent') NOT NULL DEFAULT 'Rental Agent',
    status ENUM('Active', 'Inactive') NOT NULL DEFAULT 'Active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_role (role),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Customers
-- =============================================
CREATE TABLE Customers (
    customer_id INT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100),
    phone VARCHAR(20),
    address TEXT,
    license_number VARCHAR(50),
    license_expiry DATE,
    date_of_birth DATE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_email (email),
    INDEX idx_phone (phone),
    INDEX idx_license_number (license_number),
    INDEX idx_name (first_name, last_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: VehicleCategories
-- =============================================
CREATE TABLE VehicleCategories (
    category_id INT AUTO_INCREMENT PRIMARY KEY,
    category_name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_category_name (category_name)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE Vehicles (
    vehicle_id INT AUTO_INCREMENT PRIMARY KEY,
    category_id INT NOT NULL,
    make VARCHAR(50) NOT NULL,
    model VARCHAR(50) NOT NULL,
    year INT NOT NULL,
    color VARCHAR(30),
    license_plate VARCHAR(20) NOT NULL UNIQUE,
    vin VARCHAR(50),
    mileage INT DEFAULT 0,
    fuel_type ENUM('Gasoline', 'Diesel', 'Electric', 'Hybrid'),
    transmission ENUM('Manual', 'Automatic'),
    seating_capacity INT,
    status ENUM('Available', 'Rented', 'Reserved', 'Maintenance', 'OutOfService', 'Retired') NOT NULL DEFAULT 'Available',
    daily_rate DECIMAL(10, 2) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES VehicleCategories(category_id) ON DELETE RESTRICT,
    INDEX idx_category_id (category_id),
    INDEX idx_license_plate (license_plate),
    INDEX idx_status (status),
    INDEX idx_make_model (make, model),
    INDEX idx_vin (vin)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: VehicleFeatures (lookup) & Mapping
-- =============================================
CREATE TABLE VehicleFeatures (
    feature_id INT AUTO_INCREMENT PRIMARY KEY,
    feature_name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE VehicleFeatureMap (
    vehicle_id INT NOT NULL,
    feature_id INT NOT NULL,
    PRIMARY KEY (vehicle_id, feature_id),
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id) ON DELETE CASCADE,
    FOREIGN KEY (feature_id) REFERENCES VehicleFeatures(feature_id) ON DELETE CASCADE,
    INDEX idx_vehicle_id (vehicle_id),
    INDEX idx_feature_id (feature_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: VehicleImages (store in DB)
-- =============================================
CREATE TABLE VehicleImages (
    image_id INT AUTO_INCREMENT PRIMARY KEY,
    vehicle_id INT NOT NULL,
    image_data LONGBLOB NOT NULL,
    content_type VARCHAR(100),
    caption VARCHAR(255),
    is_primary BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id) ON DELETE CASCADE,
    INDEX idx_vehicle_id (vehicle_id),
    INDEX idx_is_primary (is_primary)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: RentalRates
-- =============================================
CREATE TABLE RentalRates (
    rate_id INT AUTO_INCREMENT PRIMARY KEY,
    category_id INT NOT NULL,
    rate_name VARCHAR(100) NOT NULL,
    daily_rate DECIMAL(10, 2) NOT NULL,
    weekly_rate DECIMAL(10, 2),
    monthly_rate DECIMAL(10, 2),
    effective_from DATE NOT NULL,
    effective_to DATE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES VehicleCategories(category_id) ON DELETE RESTRICT,
    INDEX idx_category_id (category_id),
    INDEX idx_effective_from (effective_from),
    INDEX idx_effective_to (effective_to),
    INDEX idx_is_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Reservations
-- =============================================
CREATE TABLE Reservations (
    reservation_id INT AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    rate_id INT NOT NULL,
    created_by INT NOT NULL,
    reservation_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    pickup_date DATETIME NOT NULL,
    return_date DATETIME NOT NULL,
    status ENUM('Pending', 'Confirmed', 'Cancelled', 'Completed') NOT NULL DEFAULT 'Pending',
    total_amount DECIMAL(10, 2) NOT NULL,
    notes TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id) ON DELETE RESTRICT,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id) ON DELETE RESTRICT,
    FOREIGN KEY (rate_id) REFERENCES RentalRates(rate_id) ON DELETE RESTRICT,
    FOREIGN KEY (created_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    INDEX idx_customer_id (customer_id),
    INDEX idx_vehicle_id (vehicle_id),
    INDEX idx_rate_id (rate_id),
    INDEX idx_created_by (created_by),
    INDEX idx_pickup_date (pickup_date),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Rentals
-- =============================================
CREATE TABLE Rentals (
    rental_id INT AUTO_INCREMENT PRIMARY KEY,
    reservation_id INT,
    customer_id INT NOT NULL,
    vehicle_id INT NOT NULL,
    picked_up_by INT NOT NULL,
    returned_to INT,
    pickup_date DATETIME NOT NULL,
    expected_return_date DATETIME NOT NULL,
    actual_return_date DATETIME,
    initial_mileage INT,
    return_mileage INT,
    status ENUM('Active', 'Completed', 'Overdue') NOT NULL DEFAULT 'Active',
    total_amount DECIMAL(10, 2) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (reservation_id) REFERENCES Reservations(reservation_id) ON DELETE SET NULL,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id) ON DELETE RESTRICT,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id) ON DELETE RESTRICT,
    FOREIGN KEY (picked_up_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    FOREIGN KEY (returned_to) REFERENCES Users(user_id) ON DELETE SET NULL,
    INDEX idx_reservation_id (reservation_id),
    INDEX idx_customer_id (customer_id),
    INDEX idx_vehicle_id (vehicle_id),
    INDEX idx_picked_up_by (picked_up_by),
    INDEX idx_returned_to (returned_to),
    INDEX idx_pickup_date (pickup_date),
    INDEX idx_expected_return_date (expected_return_date),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: RentalCharges (line items per rental return)
-- =============================================
CREATE TABLE RentalCharges (
    charge_id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT NOT NULL,
    charge_type ENUM('LateFee', 'MileageOverage', 'Fuel', 'Cleaning', 'Damage', 'Toll', 'Other') NOT NULL,
    description VARCHAR(255),
    quantity DECIMAL(10,2) DEFAULT 1,
    unit_amount DECIMAL(10,2) NOT NULL,
    total_amount DECIMAL(10,2) GENERATED ALWAYS AS (quantity * unit_amount) STORED,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES Rentals(rental_id) ON DELETE CASCADE,
    INDEX idx_rental_id (rental_id),
    INDEX idx_charge_type (charge_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Invoices
-- =============================================
CREATE TABLE Invoices (
    invoice_id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT NOT NULL,
    invoice_number VARCHAR(50) NOT NULL UNIQUE,
    subtotal DECIMAL(10,2) NOT NULL,
    taxes DECIMAL(10,2) DEFAULT 0,
    discounts DECIMAL(10,2) DEFAULT 0,
    total DECIMAL(10,2) NOT NULL,
    balance_due DECIMAL(10,2) NOT NULL,
    issued_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    issued_by INT NOT NULL,
    FOREIGN KEY (rental_id) REFERENCES Rentals(rental_id) ON DELETE CASCADE,
    FOREIGN KEY (issued_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    INDEX idx_rental_id (rental_id),
    INDEX idx_invoice_number (invoice_number)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Tables: Rental Inspections (pickup/return) with checklist & photos
-- =============================================
CREATE TABLE RentalInspections (
    inspection_id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT NOT NULL,
    inspection_type ENUM('Pickup', 'Return') NOT NULL,
    inspected_by INT NOT NULL,
    odometer INT,
    fuel_level_percent TINYINT,
    cleanliness_rating TINYINT,
    notes TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES Rentals(rental_id) ON DELETE CASCADE,
    FOREIGN KEY (inspected_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    INDEX idx_rental_id (rental_id),
    INDEX idx_inspection_type (inspection_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE RentalInspectionItems (
    item_id INT AUTO_INCREMENT PRIMARY KEY,
    inspection_id INT NOT NULL,
    item_label VARCHAR(100) NOT NULL,
    item_status ENUM('OK', 'Issue', 'N/A') NOT NULL DEFAULT 'OK',
    notes VARCHAR(255),
    FOREIGN KEY (inspection_id) REFERENCES RentalInspections(inspection_id) ON DELETE CASCADE,
    INDEX idx_inspection_id (inspection_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE RentalInspectionPhotos (
    photo_id INT AUTO_INCREMENT PRIMARY KEY,
    inspection_id INT NOT NULL,
    photo_data LONGBLOB NOT NULL,
    content_type VARCHAR(100),
    caption VARCHAR(255),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (inspection_id) REFERENCES RentalInspections(inspection_id) ON DELETE CASCADE,
    INDEX idx_inspection_id (inspection_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Payments
-- =============================================
CREATE TABLE Payments (
    payment_id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT,
    reservation_id INT,
    payment_type ENUM('Deposit', 'Full Payment', 'Refund', 'Additional Charge') NOT NULL,
    payment_method ENUM('Cash', 'Credit Card', 'Debit Card', 'Bank Transfer') NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    payment_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    transaction_reference VARCHAR(100),
    status ENUM('Pending', 'Completed', 'Failed', 'Refunded') NOT NULL DEFAULT 'Pending',
    processed_by INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES Rentals(rental_id) ON DELETE SET NULL,
    FOREIGN KEY (reservation_id) REFERENCES Reservations(reservation_id) ON DELETE SET NULL,
    FOREIGN KEY (processed_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    INDEX idx_rental_id (rental_id),
    INDEX idx_reservation_id (reservation_id),
    INDEX idx_processed_by (processed_by),
    INDEX idx_payment_date (payment_date),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: DamageReports
-- =============================================
CREATE TABLE DamageReports (
    damage_id INT AUTO_INCREMENT PRIMARY KEY,
    rental_id INT NOT NULL,
    reported_by INT NOT NULL,
    damage_description TEXT NOT NULL,
    damage_location VARCHAR(100),
    estimated_cost DECIMAL(10, 2),
    status ENUM('Reported', 'Under Review', 'Approved', 'Rejected', 'Repaired') NOT NULL DEFAULT 'Reported',
    approved_by INT,
    approval_date DATETIME,
    repair_date DATETIME,
    actual_repair_cost DECIMAL(10, 2),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (rental_id) REFERENCES Rentals(rental_id) ON DELETE RESTRICT,
    FOREIGN KEY (reported_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    FOREIGN KEY (approved_by) REFERENCES Users(user_id) ON DELETE SET NULL,
    INDEX idx_rental_id (rental_id),
    INDEX idx_reported_by (reported_by),
    INDEX idx_approved_by (approved_by),
    INDEX idx_status (status),
    INDEX idx_created_at (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Damage report photos (for evidence)
CREATE TABLE DamageReportPhotos (
    photo_id INT AUTO_INCREMENT PRIMARY KEY,
    damage_id INT NOT NULL,
    photo_data LONGBLOB NOT NULL,
    content_type VARCHAR(100),
    caption VARCHAR(255),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (damage_id) REFERENCES DamageReports(damage_id) ON DELETE CASCADE,
    INDEX idx_damage_id (damage_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: MaintenanceRecords
-- =============================================
CREATE TABLE MaintenanceRecords (
    maintenance_id INT AUTO_INCREMENT PRIMARY KEY,
    vehicle_id INT NOT NULL,
    maintenance_type ENUM('Regular Service', 'Repair', 'Inspection', 'Oil Change') NOT NULL,
    description TEXT,
    cost DECIMAL(10, 2),
    service_date DATE NOT NULL,
    next_service_date DATE,
    service_provider VARCHAR(100),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (vehicle_id) REFERENCES Vehicles(vehicle_id) ON DELETE RESTRICT,
    INDEX idx_vehicle_id (vehicle_id),
    INDEX idx_service_date (service_date),
    INDEX idx_maintenance_type (maintenance_type)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Maintenance photos
CREATE TABLE MaintenancePhotos (
    photo_id INT AUTO_INCREMENT PRIMARY KEY,
    maintenance_id INT NOT NULL,
    photo_data LONGBLOB NOT NULL,
    content_type VARCHAR(100),
    caption VARCHAR(255),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (maintenance_id) REFERENCES MaintenanceRecords(maintenance_id) ON DELETE CASCADE,
    INDEX idx_maintenance_id (maintenance_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Table: Reports
-- =============================================
CREATE TABLE Reports (
    report_id INT AUTO_INCREMENT PRIMARY KEY,
    generated_by INT NOT NULL,
    report_type ENUM('Revenue', 'Vehicle Utilization', 'Customer', 'Damage', 'Maintenance') NOT NULL,
    report_date DATE NOT NULL,
    report_data JSON,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (generated_by) REFERENCES Users(user_id) ON DELETE RESTRICT,
    INDEX idx_generated_by (generated_by),
    INDEX idx_report_type (report_type),
    INDEX idx_report_date (report_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- STORED PROCEDURES FOR CRUD OPERATIONS
-- =============================================

DELIMITER //

-- =============================================
-- USERS CRUD PROCEDURES
-- =============================================

-- Create User
CREATE PROCEDURE sp_CreateUser(
    IN p_username VARCHAR(50),
    IN p_password_hash VARCHAR(255),
    IN p_email VARCHAR(100),
    IN p_first_name VARCHAR(50),
    IN p_last_name VARCHAR(50),
    IN p_phone VARCHAR(20),
    IN p_role ENUM('Admin', 'Rental Agent'),
    IN p_status ENUM('Active', 'Inactive')
)
BEGIN
    INSERT INTO Users (username, password_hash, email, first_name, last_name, phone, role, status)
    VALUES (p_username, p_password_hash, p_email, p_first_name, p_last_name, p_phone, p_role, p_status);
    
    SELECT LAST_INSERT_ID() AS user_id;
END //

-- Read User
CREATE PROCEDURE sp_GetUser(
    IN p_user_id INT
)
BEGIN
    SELECT user_id, username, email, first_name, last_name, phone, role, status, created_at, updated_at
    FROM Users
    WHERE user_id = p_user_id;
END //

-- Read All Users
CREATE PROCEDURE sp_GetAllUsers()
BEGIN
    SELECT user_id, username, email, first_name, last_name, phone, role, status, created_at, updated_at
    FROM Users
    ORDER BY last_name, first_name;
END //

-- Update User
CREATE PROCEDURE sp_UpdateUser(
    IN p_user_id INT,
    IN p_username VARCHAR(50),
    IN p_password_hash VARCHAR(255),
    IN p_email VARCHAR(100),
    IN p_first_name VARCHAR(50),
    IN p_last_name VARCHAR(50),
    IN p_phone VARCHAR(20),
    IN p_role ENUM('Admin', 'Rental Agent'),
    IN p_status ENUM('Active', 'Inactive')
)
BEGIN
    UPDATE Users
    SET username = p_username,
        password_hash = IFNULL(p_password_hash, password_hash),
        email = p_email,
        first_name = p_first_name,
        last_name = p_last_name,
        phone = p_phone,
        role = p_role,
        status = p_status
    WHERE user_id = p_user_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete User
CREATE PROCEDURE sp_DeleteUser(
    IN p_user_id INT
)
BEGIN
    UPDATE Users SET status = 'Inactive' WHERE user_id = p_user_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- CUSTOMERS CRUD PROCEDURES
-- =============================================

-- Create Customer
CREATE PROCEDURE sp_CreateCustomer(
    IN p_first_name VARCHAR(50),
    IN p_last_name VARCHAR(50),
    IN p_email VARCHAR(100),
    IN p_phone VARCHAR(20),
    IN p_address TEXT,
    IN p_license_number VARCHAR(50),
    IN p_license_expiry DATE,
    IN p_date_of_birth DATE
)
BEGIN
    INSERT INTO Customers (first_name, last_name, email, phone, address, license_number, license_expiry, date_of_birth)
    VALUES (p_first_name, p_last_name, p_email, p_phone, p_address, p_license_number, p_license_expiry, p_date_of_birth);
    
    SELECT LAST_INSERT_ID() AS customer_id;
END //

-- Read Customer
CREATE PROCEDURE sp_GetCustomer(
    IN p_customer_id INT
)
BEGIN
    SELECT * FROM Customers WHERE customer_id = p_customer_id;
END //

-- Read All Customers
CREATE PROCEDURE sp_GetAllCustomers()
BEGIN
    SELECT * FROM Customers ORDER BY last_name, first_name;
END //

-- Update Customer
CREATE PROCEDURE sp_UpdateCustomer(
    IN p_customer_id INT,
    IN p_first_name VARCHAR(50),
    IN p_last_name VARCHAR(50),
    IN p_email VARCHAR(100),
    IN p_phone VARCHAR(20),
    IN p_address TEXT,
    IN p_license_number VARCHAR(50),
    IN p_license_expiry DATE,
    IN p_date_of_birth DATE
)
BEGIN
    UPDATE Customers
    SET first_name = p_first_name,
        last_name = p_last_name,
        email = p_email,
        phone = p_phone,
        address = p_address,
        license_number = p_license_number,
        license_expiry = p_license_expiry,
        date_of_birth = p_date_of_birth
    WHERE customer_id = p_customer_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Customer
CREATE PROCEDURE sp_DeleteCustomer(
    IN p_customer_id INT
)
BEGIN
    DELETE FROM Customers WHERE customer_id = p_customer_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- VEHICLE CATEGORIES CRUD PROCEDURES
-- =============================================

-- Create Vehicle Category
CREATE PROCEDURE sp_CreateVehicleCategory(
    IN p_category_name VARCHAR(50),
    IN p_description TEXT
)
BEGIN
    INSERT INTO VehicleCategories (category_name, description)
    VALUES (p_category_name, p_description);
    
    SELECT LAST_INSERT_ID() AS category_id;
END //

-- Read Vehicle Category
CREATE PROCEDURE sp_GetVehicleCategory(
    IN p_category_id INT
)
BEGIN
    SELECT * FROM VehicleCategories WHERE category_id = p_category_id;
END //

-- Read All Vehicle Categories
CREATE PROCEDURE sp_GetAllVehicleCategories()
BEGIN
    SELECT * FROM VehicleCategories ORDER BY category_name;
END //

-- Update Vehicle Category
CREATE PROCEDURE sp_UpdateVehicleCategory(
    IN p_category_id INT,
    IN p_category_name VARCHAR(50),
    IN p_description TEXT
)
BEGIN
    UPDATE VehicleCategories
    SET category_name = p_category_name,
        description = p_description
    WHERE category_id = p_category_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Vehicle Category
CREATE PROCEDURE sp_DeleteVehicleCategory(
    IN p_category_id INT
)
BEGIN
    DELETE FROM VehicleCategories WHERE category_id = p_category_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- VEHICLES CRUD PROCEDURES
-- =============================================

-- Create Vehicle
CREATE PROCEDURE sp_CreateVehicle(
    IN p_category_id INT,
    IN p_make VARCHAR(50),
    IN p_model VARCHAR(50),
    IN p_year INT,
    IN p_color VARCHAR(30),
    IN p_license_plate VARCHAR(20),
    IN p_vin VARCHAR(50),
    IN p_mileage INT,
    IN p_fuel_type ENUM('Gasoline', 'Diesel', 'Electric', 'Hybrid'),
    IN p_transmission ENUM('Manual', 'Automatic'),
    IN p_seating_capacity INT,
    IN p_status ENUM('Available', 'Rented', 'Reserved', 'Maintenance', 'OutOfService', 'Retired'),
    IN p_daily_rate DECIMAL(10, 2)
)
BEGIN
    INSERT INTO Vehicles (category_id, make, model, year, color, license_plate, vin, mileage, fuel_type, transmission, seating_capacity, status, daily_rate)
    VALUES (p_category_id, p_make, p_model, p_year, p_color, p_license_plate, p_vin, p_mileage, p_fuel_type, p_transmission, p_seating_capacity, p_status, p_daily_rate);
    
    SELECT LAST_INSERT_ID() AS vehicle_id;
END //

-- Read Vehicle
CREATE PROCEDURE sp_GetVehicle(
    IN p_vehicle_id INT
)
BEGIN
    SELECT v.*, vc.category_name, vc.description AS category_description
    FROM Vehicles v
    INNER JOIN VehicleCategories vc ON v.category_id = vc.category_id
    WHERE v.vehicle_id = p_vehicle_id;
END //

-- Read All Vehicles
CREATE PROCEDURE sp_GetAllVehicles()
BEGIN
    SELECT v.*, vc.category_name, vc.description AS category_description
    FROM Vehicles v
    INNER JOIN VehicleCategories vc ON v.category_id = vc.category_id
    ORDER BY v.make, v.model;
END //

-- Available vehicles for a date range (excludes overlapping rentals/reservations/maintenance)
CREATE PROCEDURE sp_GetAvailableVehiclesForRange(
    IN p_start DATETIME,
    IN p_end DATETIME
)
BEGIN
    SELECT v.*
    FROM Vehicles v
    WHERE v.status IN ('Available', 'Reserved')
      AND v.vehicle_id NOT IN (
          SELECT vehicle_id FROM Rentals r
          WHERE r.pickup_date <= p_end
            AND (r.actual_return_date IS NULL OR r.actual_return_date >= p_start)
      )
      AND v.vehicle_id NOT IN (
          SELECT vehicle_id FROM Reservations res
          WHERE res.status IN ('Pending','Confirmed')
            AND res.pickup_date <= p_end
            AND res.return_date >= p_start
      )
      AND v.vehicle_id NOT IN (
          SELECT vehicle_id FROM MaintenanceRecords m
          WHERE m.service_date <= p_end
            AND (m.next_service_date IS NULL OR m.next_service_date >= p_start)
      )
    ORDER BY v.make, v.model, v.license_plate;
END //

-- Read Available Vehicles
CREATE PROCEDURE sp_GetAvailableVehicles()
BEGIN
    SELECT v.*, vc.category_name, vc.description AS category_description
    FROM Vehicles v
    INNER JOIN VehicleCategories vc ON v.category_id = vc.category_id
    WHERE v.status = 'Available'
    ORDER BY v.make, v.model;
END //

-- Update Vehicle
CREATE PROCEDURE sp_UpdateVehicle(
    IN p_vehicle_id INT,
    IN p_category_id INT,
    IN p_make VARCHAR(50),
    IN p_model VARCHAR(50),
    IN p_year INT,
    IN p_color VARCHAR(30),
    IN p_license_plate VARCHAR(20),
    IN p_vin VARCHAR(50),
    IN p_mileage INT,
    IN p_fuel_type ENUM('Gasoline', 'Diesel', 'Electric', 'Hybrid'),
    IN p_transmission ENUM('Manual', 'Automatic'),
    IN p_seating_capacity INT,
    IN p_status ENUM('Available', 'Rented', 'Reserved', 'Maintenance', 'OutOfService', 'Retired'),
    IN p_daily_rate DECIMAL(10, 2)
)
BEGIN
    UPDATE Vehicles
    SET category_id = p_category_id,
        make = p_make,
        model = p_model,
        year = p_year,
        color = p_color,
        license_plate = p_license_plate,
        vin = p_vin,
        mileage = p_mileage,
        fuel_type = p_fuel_type,
        transmission = p_transmission,
        seating_capacity = p_seating_capacity,
        status = p_status,
        daily_rate = p_daily_rate
    WHERE vehicle_id = p_vehicle_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Vehicle
CREATE PROCEDURE sp_DeleteVehicle(
    IN p_vehicle_id INT
)
BEGIN
    UPDATE Vehicles SET status = 'Retired' WHERE vehicle_id = p_vehicle_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- RENTAL RATES CRUD PROCEDURES
-- =============================================

-- Create Rental Rate
CREATE PROCEDURE sp_CreateRentalRate(
    IN p_category_id INT,
    IN p_rate_name VARCHAR(100),
    IN p_daily_rate DECIMAL(10, 2),
    IN p_weekly_rate DECIMAL(10, 2),
    IN p_monthly_rate DECIMAL(10, 2),
    IN p_effective_from DATE,
    IN p_effective_to DATE,
    IN p_is_active BOOLEAN
)
BEGIN
    INSERT INTO RentalRates (category_id, rate_name, daily_rate, weekly_rate, monthly_rate, effective_from, effective_to, is_active)
    VALUES (p_category_id, p_rate_name, p_daily_rate, p_weekly_rate, p_monthly_rate, p_effective_from, p_effective_to, p_is_active);
    
    SELECT LAST_INSERT_ID() AS rate_id;
END //

-- Read Rental Rate
CREATE PROCEDURE sp_GetRentalRate(
    IN p_rate_id INT
)
BEGIN
    SELECT rr.*, vc.category_name
    FROM RentalRates rr
    INNER JOIN VehicleCategories vc ON rr.category_id = vc.category_id
    WHERE rr.rate_id = p_rate_id;
END //

-- Read All Rental Rates
CREATE PROCEDURE sp_GetAllRentalRates()
BEGIN
    SELECT rr.*, vc.category_name
    FROM RentalRates rr
    INNER JOIN VehicleCategories vc ON rr.category_id = vc.category_id
    ORDER BY vc.category_name, rr.rate_name;
END //

-- Read Rental Rates by Category
CREATE PROCEDURE sp_GetRentalRatesByCategory(
    IN p_category_id INT
)
BEGIN
    SELECT rr.*, vc.category_name
    FROM RentalRates rr
    INNER JOIN VehicleCategories vc ON rr.category_id = vc.category_id
    WHERE rr.category_id = p_category_id AND rr.is_active = TRUE
    ORDER BY rr.rate_name;
END //

-- Update Rental Rate
CREATE PROCEDURE sp_UpdateRentalRate(
    IN p_rate_id INT,
    IN p_category_id INT,
    IN p_rate_name VARCHAR(100),
    IN p_daily_rate DECIMAL(10, 2),
    IN p_weekly_rate DECIMAL(10, 2),
    IN p_monthly_rate DECIMAL(10, 2),
    IN p_effective_from DATE,
    IN p_effective_to DATE,
    IN p_is_active BOOLEAN
)
BEGIN
    UPDATE RentalRates
    SET category_id = p_category_id,
        rate_name = p_rate_name,
        daily_rate = p_daily_rate,
        weekly_rate = p_weekly_rate,
        monthly_rate = p_monthly_rate,
        effective_from = p_effective_from,
        effective_to = p_effective_to,
        is_active = p_is_active
    WHERE rate_id = p_rate_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Rental Rate
CREATE PROCEDURE sp_DeleteRentalRate(
    IN p_rate_id INT
)
BEGIN
    UPDATE RentalRates SET is_active = FALSE WHERE rate_id = p_rate_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- RESERVATIONS CRUD PROCEDURES
-- =============================================

-- Create Reservation
CREATE PROCEDURE sp_CreateReservation(
    IN p_customer_id INT,
    IN p_vehicle_id INT,
    IN p_rate_id INT,
    IN p_created_by INT,
    IN p_pickup_date DATETIME,
    IN p_return_date DATETIME,
    IN p_total_amount DECIMAL(10, 2),
    IN p_notes TEXT
)
BEGIN
    INSERT INTO Reservations (customer_id, vehicle_id, rate_id, created_by, pickup_date, return_date, total_amount, notes, status)
    VALUES (p_customer_id, p_vehicle_id, p_rate_id, p_created_by, p_pickup_date, p_return_date, p_total_amount, p_notes, 'Pending');
    
    SELECT LAST_INSERT_ID() AS reservation_id;
END //

-- Read Reservation
CREATE PROCEDURE sp_GetReservation(
    IN p_reservation_id INT
)
BEGIN
    SELECT r.*,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           c.email AS customer_email,
           c.phone AS customer_phone,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate,
           rr.rate_name,
           u.first_name AS created_by_first_name,
           u.last_name AS created_by_last_name
    FROM Reservations r
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    INNER JOIN RentalRates rr ON r.rate_id = rr.rate_id
    INNER JOIN Users u ON r.created_by = u.user_id
    WHERE r.reservation_id = p_reservation_id;
END //

-- Read All Reservations
CREATE PROCEDURE sp_GetAllReservations()
BEGIN
    SELECT r.*,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM Reservations r
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    ORDER BY r.reservation_date DESC;
END //

-- Update Reservation
CREATE PROCEDURE sp_UpdateReservation(
    IN p_reservation_id INT,
    IN p_pickup_date DATETIME,
    IN p_return_date DATETIME,
    IN p_status ENUM('Pending', 'Confirmed', 'Cancelled', 'Completed'),
    IN p_total_amount DECIMAL(10, 2),
    IN p_notes TEXT
)
BEGIN
    UPDATE Reservations
    SET pickup_date = p_pickup_date,
        return_date = p_return_date,
        status = p_status,
        total_amount = p_total_amount,
        notes = p_notes
    WHERE reservation_id = p_reservation_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Reservation
CREATE PROCEDURE sp_DeleteReservation(
    IN p_reservation_id INT
)
BEGIN
    DELETE FROM Reservations WHERE reservation_id = p_reservation_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- RENTALS CRUD PROCEDURES
-- =============================================

-- Create Rental
CREATE PROCEDURE sp_CreateRental(
    IN p_reservation_id INT,
    IN p_customer_id INT,
    IN p_vehicle_id INT,
    IN p_picked_up_by INT,
    IN p_pickup_date DATETIME,
    IN p_expected_return_date DATETIME,
    IN p_initial_mileage INT,
    IN p_total_amount DECIMAL(10, 2)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    INSERT INTO Rentals (reservation_id, customer_id, vehicle_id, picked_up_by, pickup_date, expected_return_date, initial_mileage, total_amount, status)
    VALUES (p_reservation_id, p_customer_id, p_vehicle_id, p_picked_up_by, p_pickup_date, p_expected_return_date, p_initial_mileage, p_total_amount, 'Active');
    
    UPDATE Vehicles SET status = 'Rented' WHERE vehicle_id = p_vehicle_id;
    
    IF p_reservation_id IS NOT NULL THEN
        UPDATE Reservations SET status = 'Completed' WHERE reservation_id = p_reservation_id;
    END IF;
    
    COMMIT;
    
    SELECT LAST_INSERT_ID() AS rental_id;
END //

-- Read Rental
CREATE PROCEDURE sp_GetRental(
    IN p_rental_id INT
)
BEGIN
    SELECT r.*,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           c.email AS customer_email,
           c.phone AS customer_phone,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate,
           u1.first_name AS picked_up_by_first_name,
           u1.last_name AS picked_up_by_last_name,
           u2.first_name AS returned_to_first_name,
           u2.last_name AS returned_to_last_name
    FROM Rentals r
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    INNER JOIN Users u1 ON r.picked_up_by = u1.user_id
    LEFT JOIN Users u2 ON r.returned_to = u2.user_id
    WHERE r.rental_id = p_rental_id;
END //

-- Read All Rentals
CREATE PROCEDURE sp_GetAllRentals()
BEGIN
    SELECT r.*,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM Rentals r
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    ORDER BY r.pickup_date DESC;
END //

-- Read Active Rentals
CREATE PROCEDURE sp_GetActiveRentals()
BEGIN
    SELECT r.*,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM Rentals r
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    WHERE r.status = 'Active'
    ORDER BY r.expected_return_date;
END //

-- Update Rental
CREATE PROCEDURE sp_UpdateRental(
    IN p_rental_id INT,
    IN p_expected_return_date DATETIME,
    IN p_actual_return_date DATETIME,
    IN p_return_mileage INT,
    IN p_returned_to INT,
    IN p_status ENUM('Active', 'Completed', 'Overdue'),
    IN p_total_amount DECIMAL(10, 2)
)
BEGIN
    DECLARE v_vehicle_id INT;
    DECLARE v_old_status VARCHAR(20);
    
    SELECT vehicle_id, status INTO v_vehicle_id, v_old_status
    FROM Rentals WHERE rental_id = p_rental_id;
    
    START TRANSACTION;
    
    UPDATE Rentals
    SET expected_return_date = p_expected_return_date,
        actual_return_date = p_actual_return_date,
        return_mileage = p_return_mileage,
        returned_to = p_returned_to,
        status = p_status,
        total_amount = p_total_amount
    WHERE rental_id = p_rental_id;
    
    IF p_status = 'Completed' THEN
        UPDATE Vehicles SET status = 'Available', mileage = p_return_mileage WHERE vehicle_id = v_vehicle_id;
    ELSEIF p_status = 'Active' AND v_old_status != 'Active' THEN
        UPDATE Vehicles SET status = 'Rented' WHERE vehicle_id = v_vehicle_id;
    END IF;
    
    COMMIT;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Rental
CREATE PROCEDURE sp_DeleteRental(
    IN p_rental_id INT
)
BEGIN
    DECLARE v_vehicle_id INT;
    
    SELECT vehicle_id INTO v_vehicle_id FROM Rentals WHERE rental_id = p_rental_id;
    
    START TRANSACTION;
    
    DELETE FROM Rentals WHERE rental_id = p_rental_id;
    
    UPDATE Vehicles SET status = 'Available' WHERE vehicle_id = v_vehicle_id;
    
    COMMIT;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- PAYMENTS CRUD PROCEDURES
-- =============================================

-- Create Payment
CREATE PROCEDURE sp_CreatePayment(
    IN p_rental_id INT,
    IN p_reservation_id INT,
    IN p_payment_type ENUM('Deposit', 'Full Payment', 'Refund', 'Additional Charge'),
    IN p_payment_method ENUM('Cash', 'Credit Card', 'Debit Card', 'Bank Transfer'),
    IN p_amount DECIMAL(10, 2),
    IN p_transaction_reference VARCHAR(100),
    IN p_status ENUM('Pending', 'Completed', 'Failed', 'Refunded'),
    IN p_processed_by INT
)
BEGIN
    INSERT INTO Payments (rental_id, reservation_id, payment_type, payment_method, amount, transaction_reference, status, processed_by)
    VALUES (p_rental_id, p_reservation_id, p_payment_type, p_payment_method, p_amount, p_transaction_reference, p_status, p_processed_by);
    
    SELECT LAST_INSERT_ID() AS payment_id;
END //

-- Read Payment
CREATE PROCEDURE sp_GetPayment(
    IN p_payment_id INT
)
BEGIN
    SELECT p.*,
           r.rental_id,
           r.total_amount AS rental_total_amount,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           u.first_name AS processed_by_first_name,
           u.last_name AS processed_by_last_name
    FROM Payments p
    LEFT JOIN Rentals r ON p.rental_id = r.rental_id
    LEFT JOIN Reservations res ON p.reservation_id = res.reservation_id
    LEFT JOIN Customers c ON r.customer_id = c.customer_id OR res.customer_id = c.customer_id
    INNER JOIN Users u ON p.processed_by = u.user_id
    WHERE p.payment_id = p_payment_id;
END //

-- Read All Payments
CREATE PROCEDURE sp_GetAllPayments()
BEGIN
    SELECT p.*,
           r.rental_id,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name
    FROM Payments p
    LEFT JOIN Rentals r ON p.rental_id = r.rental_id
    LEFT JOIN Reservations res ON p.reservation_id = res.reservation_id
    LEFT JOIN Customers c ON r.customer_id = c.customer_id OR res.customer_id = c.customer_id
    ORDER BY p.payment_date DESC;
END //

-- Update Payment
CREATE PROCEDURE sp_UpdatePayment(
    IN p_payment_id INT,
    IN p_payment_type ENUM('Deposit', 'Full Payment', 'Refund', 'Additional Charge'),
    IN p_payment_method ENUM('Cash', 'Credit Card', 'Debit Card', 'Bank Transfer'),
    IN p_amount DECIMAL(10, 2),
    IN p_payment_date DATETIME,
    IN p_transaction_reference VARCHAR(100),
    IN p_status ENUM('Pending', 'Completed', 'Failed', 'Refunded')
)
BEGIN
    UPDATE Payments
    SET payment_type = p_payment_type,
        payment_method = p_payment_method,
        amount = p_amount,
        payment_date = p_payment_date,
        transaction_reference = p_transaction_reference,
        status = p_status
    WHERE payment_id = p_payment_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Payment
CREATE PROCEDURE sp_DeletePayment(
    IN p_payment_id INT
)
BEGIN
    DELETE FROM Payments WHERE payment_id = p_payment_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- DAMAGE REPORTS CRUD PROCEDURES
-- =============================================

-- Create Damage Report
CREATE PROCEDURE sp_CreateDamageReport(
    IN p_rental_id INT,
    IN p_reported_by INT,
    IN p_damage_description TEXT,
    IN p_damage_location VARCHAR(100),
    IN p_estimated_cost DECIMAL(10, 2)
)
BEGIN
    INSERT INTO DamageReports (rental_id, reported_by, damage_description, damage_location, estimated_cost, status)
    VALUES (p_rental_id, p_reported_by, p_damage_description, p_damage_location, p_estimated_cost, 'Reported');
    
    SELECT LAST_INSERT_ID() AS damage_id;
END //

-- Read Damage Report
CREATE PROCEDURE sp_GetDamageReport(
    IN p_damage_id INT
)
BEGIN
    SELECT d.*,
           r.rental_id,
           c.first_name AS customer_first_name,
           c.last_name AS customer_last_name,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           u1.first_name AS reported_by_first_name,
           u1.last_name AS reported_by_last_name,
           u2.first_name AS approved_by_first_name,
           u2.last_name AS approved_by_last_name
    FROM DamageReports d
    INNER JOIN Rentals r ON d.rental_id = r.rental_id
    INNER JOIN Customers c ON r.customer_id = c.customer_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    INNER JOIN Users u1 ON d.reported_by = u1.user_id
    LEFT JOIN Users u2 ON d.approved_by = u2.user_id
    WHERE d.damage_id = p_damage_id;
END //

-- Read All Damage Reports
CREATE PROCEDURE sp_GetAllDamageReports()
BEGIN
    SELECT d.*,
           r.rental_id,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM DamageReports d
    INNER JOIN Rentals r ON d.rental_id = r.rental_id
    INNER JOIN Vehicles v ON r.vehicle_id = v.vehicle_id
    ORDER BY d.created_at DESC;
END //

-- Update Damage Report
CREATE PROCEDURE sp_UpdateDamageReport(
    IN p_damage_id INT,
    IN p_damage_description TEXT,
    IN p_damage_location VARCHAR(100),
    IN p_estimated_cost DECIMAL(10, 2),
    IN p_status ENUM('Reported', 'Under Review', 'Approved', 'Rejected', 'Repaired'),
    IN p_approved_by INT,
    IN p_approval_date DATETIME,
    IN p_repair_date DATETIME,
    IN p_actual_repair_cost DECIMAL(10, 2)
)
BEGIN
    UPDATE DamageReports
    SET damage_description = p_damage_description,
        damage_location = p_damage_location,
        estimated_cost = p_estimated_cost,
        status = p_status,
        approved_by = p_approved_by,
        approval_date = p_approval_date,
        repair_date = p_repair_date,
        actual_repair_cost = p_actual_repair_cost
    WHERE damage_id = p_damage_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Damage Report
CREATE PROCEDURE sp_DeleteDamageReport(
    IN p_damage_id INT
)
BEGIN
    DELETE FROM DamageReports WHERE damage_id = p_damage_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- MAINTENANCE RECORDS CRUD PROCEDURES
-- =============================================

-- Create Maintenance Record
CREATE PROCEDURE sp_CreateMaintenanceRecord(
    IN p_vehicle_id INT,
    IN p_maintenance_type ENUM('Regular Service', 'Repair', 'Inspection', 'Oil Change'),
    IN p_description TEXT,
    IN p_cost DECIMAL(10, 2),
    IN p_service_date DATE,
    IN p_next_service_date DATE,
    IN p_service_provider VARCHAR(100)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;
    
    START TRANSACTION;
    
    INSERT INTO MaintenanceRecords (vehicle_id, maintenance_type, description, cost, service_date, next_service_date, service_provider)
    VALUES (p_vehicle_id, p_maintenance_type, p_description, p_cost, p_service_date, p_next_service_date, p_service_provider);
    
    UPDATE Vehicles SET status = 'Maintenance' WHERE vehicle_id = p_vehicle_id;
    
    COMMIT;
    
    SELECT LAST_INSERT_ID() AS maintenance_id;
END //

-- Read Maintenance Record
CREATE PROCEDURE sp_GetMaintenanceRecord(
    IN p_maintenance_id INT
)
BEGIN
    SELECT m.*,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM MaintenanceRecords m
    INNER JOIN Vehicles v ON m.vehicle_id = v.vehicle_id
    WHERE m.maintenance_id = p_maintenance_id;
END //

-- Read All Maintenance Records
CREATE PROCEDURE sp_GetAllMaintenanceRecords()
BEGIN
    SELECT m.*,
           v.make AS vehicle_make,
           v.model AS vehicle_model,
           v.license_plate
    FROM MaintenanceRecords m
    INNER JOIN Vehicles v ON m.vehicle_id = v.vehicle_id
    ORDER BY m.service_date DESC;
END //

-- Update Maintenance Record
CREATE PROCEDURE sp_UpdateMaintenanceRecord(
    IN p_maintenance_id INT,
    IN p_vehicle_id INT,
    IN p_maintenance_type ENUM('Regular Service', 'Repair', 'Inspection', 'Oil Change'),
    IN p_description TEXT,
    IN p_cost DECIMAL(10, 2),
    IN p_service_date DATE,
    IN p_next_service_date DATE,
    IN p_service_provider VARCHAR(100)
)
BEGIN
    UPDATE MaintenanceRecords
    SET vehicle_id = p_vehicle_id,
        maintenance_type = p_maintenance_type,
        description = p_description,
        cost = p_cost,
        service_date = p_service_date,
        next_service_date = p_next_service_date,
        service_provider = p_service_provider
    WHERE maintenance_id = p_maintenance_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Maintenance Record
CREATE PROCEDURE sp_DeleteMaintenanceRecord(
    IN p_maintenance_id INT
)
BEGIN
    DELETE FROM MaintenanceRecords WHERE maintenance_id = p_maintenance_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- REPORTS CRUD PROCEDURES
-- =============================================

-- Create Report
CREATE PROCEDURE sp_CreateReport(
    IN p_generated_by INT,
    IN p_report_type ENUM('Revenue', 'Vehicle Utilization', 'Customer', 'Damage', 'Maintenance'),
    IN p_report_date DATE,
    IN p_report_data JSON
)
BEGIN
    INSERT INTO Reports (generated_by, report_type, report_date, report_data)
    VALUES (p_generated_by, p_report_type, p_report_date, p_report_data);
    
    SELECT LAST_INSERT_ID() AS report_id;
END //

-- Read Report
CREATE PROCEDURE sp_GetReport(
    IN p_report_id INT
)
BEGIN
    SELECT r.*,
           u.first_name AS generated_by_first_name,
           u.last_name AS generated_by_last_name
    FROM Reports r
    INNER JOIN Users u ON r.generated_by = u.user_id
    WHERE r.report_id = p_report_id;
END //

-- Read All Reports
CREATE PROCEDURE sp_GetAllReports()
BEGIN
    SELECT r.*,
           u.first_name AS generated_by_first_name,
           u.last_name AS generated_by_last_name
    FROM Reports r
    INNER JOIN Users u ON r.generated_by = u.user_id
    ORDER BY r.report_date DESC, r.created_at DESC;
END //

-- Update Report
CREATE PROCEDURE sp_UpdateReport(
    IN p_report_id INT,
    IN p_report_type ENUM('Revenue', 'Vehicle Utilization', 'Customer', 'Damage', 'Maintenance'),
    IN p_report_date DATE,
    IN p_report_data JSON
)
BEGIN
    UPDATE Reports
    SET report_type = p_report_type,
        report_date = p_report_date,
        report_data = p_report_data
    WHERE report_id = p_report_id;
    
    SELECT ROW_COUNT() AS affected_rows;
END //

-- Delete Report
CREATE PROCEDURE sp_DeleteReport(
    IN p_report_id INT
)
BEGIN
    DELETE FROM Reports WHERE report_id = p_report_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- EXTENSIONS: Vehicle Images & Features
-- =============================================

CREATE PROCEDURE sp_AddVehicleFeature(
    IN p_feature_name VARCHAR(100),
    IN p_description TEXT
)
BEGIN
    INSERT IGNORE INTO VehicleFeatures (feature_name, description)
    VALUES (p_feature_name, p_description);
    SELECT LAST_INSERT_ID() AS feature_id;
END //

CREATE PROCEDURE sp_GetAllVehicleFeatures()
BEGIN
    SELECT * FROM VehicleFeatures ORDER BY feature_name;
END //

CREATE PROCEDURE sp_MapVehicleFeature(
    IN p_vehicle_id INT,
    IN p_feature_id INT
)
BEGIN
    INSERT IGNORE INTO VehicleFeatureMap (vehicle_id, feature_id)
    VALUES (p_vehicle_id, p_feature_id);
    SELECT ROW_COUNT() AS affected_rows;
END //

CREATE PROCEDURE sp_RemoveVehicleFeature(
    IN p_vehicle_id INT,
    IN p_feature_id INT
)
BEGIN
    DELETE FROM VehicleFeatureMap WHERE vehicle_id = p_vehicle_id AND feature_id = p_feature_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

CREATE PROCEDURE sp_CreateVehicleImage(
    IN p_vehicle_id INT,
    IN p_image_data LONGBLOB,
    IN p_content_type VARCHAR(100),
    IN p_caption VARCHAR(255),
    IN p_is_primary BOOLEAN
)
BEGIN
    START TRANSACTION;
    IF p_is_primary THEN
        UPDATE VehicleImages SET is_primary = FALSE WHERE vehicle_id = p_vehicle_id;
    END IF;
    INSERT INTO VehicleImages (vehicle_id, image_data, content_type, caption, is_primary)
    VALUES (p_vehicle_id, p_image_data, p_content_type, p_caption, p_is_primary);
    COMMIT;
    SELECT LAST_INSERT_ID() AS image_id;
END //

CREATE PROCEDURE sp_SetPrimaryVehicleImage(
    IN p_vehicle_id INT,
    IN p_image_id INT
)
BEGIN
    START TRANSACTION;
    UPDATE VehicleImages SET is_primary = FALSE WHERE vehicle_id = p_vehicle_id;
    UPDATE VehicleImages SET is_primary = TRUE WHERE image_id = p_image_id AND vehicle_id = p_vehicle_id;
    COMMIT;
    SELECT ROW_COUNT() AS affected_rows;
END //

CREATE PROCEDURE sp_GetVehicleImages(
    IN p_vehicle_id INT
)
BEGIN
    SELECT * FROM VehicleImages WHERE vehicle_id = p_vehicle_id ORDER BY is_primary DESC, created_at DESC;
END //

CREATE PROCEDURE sp_DeleteVehicleImage(
    IN p_image_id INT
)
BEGIN
    DELETE FROM VehicleImages WHERE image_id = p_image_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

-- =============================================
-- EXTENSIONS: Rental Charges & Invoices
-- =============================================
CREATE PROCEDURE sp_CreateRentalCharge(
    IN p_rental_id INT,
    IN p_charge_type ENUM('LateFee', 'MileageOverage', 'Fuel', 'Cleaning', 'Damage', 'Toll', 'Other'),
    IN p_description VARCHAR(255),
    IN p_quantity DECIMAL(10,2),
    IN p_unit_amount DECIMAL(10,2)
)
BEGIN
    INSERT INTO RentalCharges (rental_id, charge_type, description, quantity, unit_amount)
    VALUES (p_rental_id, p_charge_type, p_description, p_quantity, p_unit_amount);
    SELECT LAST_INSERT_ID() AS charge_id;
END //

CREATE PROCEDURE sp_GetRentalCharges(
    IN p_rental_id INT
)
BEGIN
    SELECT * FROM RentalCharges WHERE rental_id = p_rental_id ORDER BY created_at DESC;
END //

CREATE PROCEDURE sp_DeleteRentalCharge(
    IN p_charge_id INT
)
BEGIN
    DELETE FROM RentalCharges WHERE charge_id = p_charge_id;
    SELECT ROW_COUNT() AS affected_rows;
END //

CREATE PROCEDURE sp_CreateInvoice(
    IN p_rental_id INT,
    IN p_subtotal DECIMAL(10,2),
    IN p_taxes DECIMAL(10,2),
    IN p_discounts DECIMAL(10,2),
    IN p_total DECIMAL(10,2),
    IN p_balance_due DECIMAL(10,2),
    IN p_issued_by INT
)
BEGIN
    DECLARE v_invoice_number VARCHAR(50);
    SET v_invoice_number = CONCAT('INV-', UNIX_TIMESTAMP(), '-', p_rental_id);
    INSERT INTO Invoices (rental_id, invoice_number, subtotal, taxes, discounts, total, balance_due, issued_by)
    VALUES (p_rental_id, v_invoice_number, p_subtotal, p_taxes, p_discounts, p_total, p_balance_due, p_issued_by);
    SELECT LAST_INSERT_ID() AS invoice_id, v_invoice_number AS invoice_number;
END //

CREATE PROCEDURE sp_GetInvoice(
    IN p_invoice_id INT
)
BEGIN
    SELECT * FROM Invoices WHERE invoice_id = p_invoice_id;
END //

CREATE PROCEDURE sp_GetInvoiceByRental(
    IN p_rental_id INT
)
BEGIN
    SELECT * FROM Invoices WHERE rental_id = p_rental_id ORDER BY issued_at DESC LIMIT 1;
END //

-- =============================================
-- EXTENSIONS: Rental Inspections & Photos
-- =============================================
CREATE PROCEDURE sp_CreateRentalInspection(
    IN p_rental_id INT,
    IN p_inspection_type ENUM('Pickup', 'Return'),
    IN p_inspected_by INT,
    IN p_odometer INT,
    IN p_fuel_level_percent TINYINT,
    IN p_cleanliness_rating TINYINT,
    IN p_notes TEXT
)
BEGIN
    INSERT INTO RentalInspections (rental_id, inspection_type, inspected_by, odometer, fuel_level_percent, cleanliness_rating, notes)
    VALUES (p_rental_id, p_inspection_type, p_inspected_by, p_odometer, p_fuel_level_percent, p_cleanliness_rating, p_notes);
    SELECT LAST_INSERT_ID() AS inspection_id;
END //

CREATE PROCEDURE sp_CreateRentalInspectionItem(
    IN p_inspection_id INT,
    IN p_item_label VARCHAR(100),
    IN p_item_status ENUM('OK', 'Issue', 'N/A'),
    IN p_notes VARCHAR(255)
)
BEGIN
    INSERT INTO RentalInspectionItems (inspection_id, item_label, item_status, notes)
    VALUES (p_inspection_id, p_item_label, p_item_status, p_notes);
    SELECT LAST_INSERT_ID() AS item_id;
END //

CREATE PROCEDURE sp_CreateRentalInspectionPhoto(
    IN p_inspection_id INT,
    IN p_photo_data LONGBLOB,
    IN p_content_type VARCHAR(100),
    IN p_caption VARCHAR(255)
)
BEGIN
    INSERT INTO RentalInspectionPhotos (inspection_id, photo_data, content_type, caption)
    VALUES (p_inspection_id, p_photo_data, p_content_type, p_caption);
    SELECT LAST_INSERT_ID() AS photo_id;
END //

CREATE PROCEDURE sp_GetRentalInspectionsByRental(
    IN p_rental_id INT
)
BEGIN
    SELECT * FROM RentalInspections WHERE rental_id = p_rental_id ORDER BY created_at DESC;
END //

-- =============================================
-- EXTENSIONS: Damage/Maintenance Photos
-- =============================================
CREATE PROCEDURE sp_AddDamageReportPhoto(
    IN p_damage_id INT,
    IN p_photo_data LONGBLOB,
    IN p_content_type VARCHAR(100),
    IN p_caption VARCHAR(255)
)
BEGIN
    INSERT INTO DamageReportPhotos (damage_id, photo_data, content_type, caption)
    VALUES (p_damage_id, p_photo_data, p_content_type, p_caption);
    SELECT LAST_INSERT_ID() AS photo_id;
END //

CREATE PROCEDURE sp_GetDamageReportPhotos(
    IN p_damage_id INT
)
BEGIN
    SELECT * FROM DamageReportPhotos WHERE damage_id = p_damage_id ORDER BY created_at DESC;
END //

CREATE PROCEDURE sp_AddMaintenancePhoto(
    IN p_maintenance_id INT,
    IN p_photo_data LONGBLOB,
    IN p_content_type VARCHAR(100),
    IN p_caption VARCHAR(255)
)
BEGIN
    INSERT INTO MaintenancePhotos (maintenance_id, photo_data, content_type, caption)
    VALUES (p_maintenance_id, p_photo_data, p_content_type, p_caption);
    SELECT LAST_INSERT_ID() AS photo_id;
END //

CREATE PROCEDURE sp_GetMaintenancePhotos(
    IN p_maintenance_id INT
)
BEGIN
    SELECT * FROM MaintenancePhotos WHERE maintenance_id = p_maintenance_id ORDER BY created_at DESC;
END //

-- =============================================
-- EXTENSIONS: Availability Calendar (reservations + maintenance)
-- =============================================
CREATE PROCEDURE sp_GetAvailabilityCalendar(
    IN p_start DATETIME,
    IN p_end DATETIME
)
BEGIN
    SELECT 'Reservation' AS event_type, r.reservation_id AS event_id, r.vehicle_id, r.pickup_date AS start_at, r.return_date AS end_at, r.status
    FROM Reservations r
    WHERE r.pickup_date <= p_end AND r.return_date >= p_start
    UNION ALL
    SELECT 'Maintenance' AS event_type, m.maintenance_id AS event_id, m.vehicle_id, m.service_date AS start_at, IFNULL(m.next_service_date, m.service_date) AS end_at, m.maintenance_type AS status
    FROM MaintenanceRecords m
    WHERE m.service_date <= p_end AND (m.next_service_date IS NULL OR m.next_service_date >= p_start)
    UNION ALL
    SELECT 'Rental' AS event_type, rl.rental_id AS event_id, rl.vehicle_id, rl.pickup_date AS start_at, IFNULL(rl.actual_return_date, rl.expected_return_date) AS end_at, rl.status
    FROM Rentals rl
    WHERE rl.pickup_date <= p_end AND (rl.actual_return_date IS NULL OR rl.actual_return_date >= p_start);
END //

DELIMITER ;

-- =============================================
-- SAMPLE DATA (Optional - for testing)
-- =============================================

-- Insert sample vehicle categories
INSERT INTO VehicleCategories (category_name, description) VALUES
('Sedan', '4-door passenger car'),
('SUV', 'Sport Utility Vehicle'),
('Truck', 'Pickup truck'),
('Van', 'Passenger or cargo van'),
('Convertible', 'Convertible sports car');

-- Insert sample users
INSERT INTO Users (username, password_hash, email, first_name, last_name, phone, role, status) VALUES
('admin', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'admin@rental.com', 'Admin', 'User', '555-0001', 'Admin', 'Active'),
('agent1', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'agent@rental.com', 'Jane', 'Agent', '555-0003', 'Rental Agent', 'Active');

-- Insert sample rental rates
INSERT INTO RentalRates (category_id, rate_name, daily_rate, weekly_rate, monthly_rate, effective_from, is_active) VALUES
(1, 'Standard Daily Rate', 50.00, 300.00, 1200.00, CURDATE(), TRUE),
(2, 'SUV Daily Rate', 65.00, 400.00, 1500.00, CURDATE(), TRUE),
(3, 'Truck Daily Rate', 70.00, 420.00, 1600.00, CURDATE(), TRUE);

-- Insert sample vehicles
INSERT INTO Vehicles (category_id, make, model, year, color, license_plate, vin, mileage, fuel_type, transmission, seating_capacity, status, daily_rate) VALUES
(1, 'Toyota', 'Camry', 2022, 'Silver', 'ABC-1234', '1HGBH41JXMN109186', 15000, 'Gasoline', 'Automatic', 5, 'Available', 45.00),
(2, 'Honda', 'CR-V', 2023, 'Black', 'XYZ-5678', '2HGBH41JXMN109187', 8000, 'Gasoline', 'Automatic', 5, 'Available', 55.00),
(1, 'Ford', 'Fusion', 2021, 'White', 'DEF-9012', '3HGBH41JXMN109188', 25000, 'Gasoline', 'Automatic', 5, 'Available', 40.00),
(3, 'Chevrolet', 'Silverado', 2022, 'Red', 'GHI-3456', '4HGBH41JXMN109189', 12000, 'Gasoline', 'Automatic', 5, 'Available', 65.00);

-- Insert sample customers
INSERT INTO Customers (first_name, last_name, email, phone, address, license_number, license_expiry, date_of_birth) VALUES
('Alice', 'Johnson', 'alice@email.com', '555-1001', '123 Main St', 'DL123456', '2025-12-31', '1990-05-15'),
('Bob', 'Smith', 'bob@email.com', '555-1002', '456 Oak Ave', 'DL234567', '2026-06-30', '1985-08-20'),
('Carol', 'Williams', 'carol@email.com', '555-1003', '789 Pine Rd', 'DL345678', '2025-09-15', '1992-11-10');
