CREATE DATABASE MobileShopManagementSystem;

USE MobileShopManagementSystem;


CREATE TABLE dbo.login (
    UserID INT PRIMARY KEY IDENTITY(1,1), 
    Username NVARCHAR(50) NOT NULL UNIQUE, 
	SecurityAnswer NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL 
	);

	INSERT INTO dbo.login (Username, SecurityAnswer, Password)
VALUES
('Admin123', 'MyFirstPetName', 'Admin@123'), -- Example admin user (!!! CHANGE !!!)
('Tea', 'FavoriteCity', 'Juce@65');      -- Example regular user (!!! CHANGE !!!)
GO

select * from login;


CREATE TABLE mobile (
    mobile_id INT PRIMARY KEY IDENTITY(1,1), -- PRIMARY KEY inherently means NOT NULL
    brand NVARCHAR(50) NOT NULL,
    model NVARCHAR(50) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    storage NVARCHAR(20),
    ram NVARCHAR(20),
    f_cam NVARCHAR(20),
    b_cam NVARCHAR(20),
    warranty NVARCHAR(20),
    size NVARCHAR(20),
    date DATE,
    d_name NVARCHAR(150),
    d_id NVARCHAR(20),
    quantity INT
);

INSERT INTO mobile (brand, model, price, storage, ram, f_cam, b_cam, warranty, size, date, d_id, quantity)
VALUES 
('Apple', 'iPhone 14', 999.99, '128GB', '6GB', '12MP', '48MP', '1 Year', '6.1 Inch', '2023-01-01', 'D001', 50),
('Samsung', 'Galaxy S23', 899.99, '256GB', '8GB', '10MP', '50MP', '2 Years', '6.2 Inch', '2023-02-15', 'D002', 30),
('Google', 'Pixel 7', 799.99, '128GB', '8GB', '10.8MP', '50MP', '1 Year', '6.3 Inch', '2023-03-01', 'D003', 20);

EXEC sp_rename 'dbo.mobile.id', 'mobile_id', 'COLUMN';
GO


CREATE TABLE customer (
    id INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
    name NVARCHAR(100) NOT NULL,      -- Customer's name
    mobile NVARCHAR(15) NOT NULL,     -- Customer's phone number
    addr NVARCHAR(255),               -- Customer's address
    date DATE,                        -- Date of registration
    m_id INT,                         -- Mobile ID (foreign key to 'mobile' table)
    brand NVARCHAR(50),               -- Brand of the purchased mobile
    model NVARCHAR(50),               -- Model of the purchased mobile
    price DECIMAL(10, 2),             -- Price of the purchased mobile
    warranty NVARCHAR(20),            -- Warranty period of the purchased mobile
    d_id NVARCHAR(20)                 -- Dealer ID
);
INSERT INTO customer (name, mobile, addr, date, m_id, brand, model, price, warranty, d_id)
VALUES 
('John Doe', '1234567890', '123 Main St', '2023-03-01', 1, 'Apple', 'iPhone 14', 999.99, '1 Year', 'D001'),
('Jane Smith', '0987654321', '456 Elm St', '2023-03-10', 2, 'Samsung', 'Galaxy S23', 899.99, '2 Years', 'D002');

ALTER TABLE customer
ADD date_registered DATETIME DEFAULT GETDATE(); -- Or DATE if time isn't needed

ALTER TABLE customer
ADD IsActive BIT DEFAULT 1 NOT NULL;


CREATE TABLE sales (
    SaleID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing primary key
    customer_id INT NOT NULL,         -- Foreign key to 'customer' table
    mobile_id INT NOT NULL,           -- Foreign key to 'mobile' table
    sale_date DATE NOT NULL,          -- Date of sale
    quantity INT NOT NULL,            -- Quantity sold
    total_amount DECIMAL(10, 2) NOT NULL, -- Total amount of the sale
	PaymentMethod NVARCHAR(50) NOT NULL
);

EXEC sp_rename 'dbo.sales.id', 'SaleID', 'COLUMN';
GO

INSERT INTO sales (customer_id, mobile_id, sale_date, quantity, total_amount)
VALUES 
(1, 1, '2023-03-02', 1, 999.99),
(2, 2, '2023-03-11', 1, 899.99);

ALTER TABLE sale_items
ADD SaleID INT NOT NULL;

select * from sales;

CREATE TABLE [sales report] (
    ReportEntryID INT PRIMARY KEY IDENTITY(1,1), -- A new primary key for this report table itself
    OriginalSaleID INT NOT NULL,                 -- The ID from the original 'sales' table
    SaleDate DATE,
    CustomerName NVARCHAR(100),
    MobileBrand NVARCHAR(50),
    MobileModel NVARCHAR(50),
    Quantity INT,
    TotalAmount DECIMAL(10, 2)
);




CREATE TABLE dealer (
    -- Still recommended to have an internal auto-incrementing PK
    id INT PRIMARY KEY IDENTITY(1,1), -- The unique business Dealer ID (e.g., 'D001')
    dealer_id_business NVARCHAR(20) NOT NULL UNIQUE, -- Renamed from d_id for clarity
    dealer_name NVARCHAR(150) NOT NULL,
    phone_number NVARCHAR(25) NULL, -- Kept nullable as it might not always be required
    address NVARCHAR(500) NULL, -- Adjust size (e.g., 255, 500, MAX) as needed
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE payment (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),    -- Unique ID for each payment record
    SaleID INT NOT NULL,                        -- Foreign Key linking to the specific sale
    PaymentMethod NVARCHAR(50) NOT NULL,        -- How the payment was made (e.g., 'Cash', 'Card')
    AmountPaid DECIMAL(10, 2) NOT NULL,         -- The amount paid in this specific transaction
    PaymentTimestamp DATETIME2 NOT NULL DEFAULT GETDATE(), -- When the payment was recorded
    TransactionReference NVARCHAR(255) NULL,   -- Optional: ID for card/online transactions, check number, etc.
    Notes NVARCHAR(500) NULL,                   -- Optional: Any notes related to the payment

    -- Constraint to link to the sales table
    CONSTRAINT FK_payment_sales FOREIGN KEY (SaleID)
        REFERENCES sales(id)
        ON DELETE NO ACTION -- Prevent deleting a sale if payments exist (common setting)
        ON UPDATE CASCADE   -- If a sale ID were ever updated (unlikely with IDENTITY), update it here too
);
GO


CREATE TABLE sale_items (
    SaleItemID INT PRIMARY KEY IDENTITY(1,1),
    SaleID INT NOT NULL,             -- Foreign Key to the main sales header record
    mobile_id INT NOT NULL,           -- Foreign Key to the specific mobile sold
    QuantitySold INT NOT NULL,
    PriceAtSale DECIMAL(10, 2) NOT NULL, -- Price of ONE unit at the time of sale
    LineTotal AS (CAST(QuantitySold AS DECIMAL(10,2)) * PriceAtSale) PERSISTED, -- Calculated total for this line

    CONSTRAINT FK_SaleItems_Sales FOREIGN KEY (SaleID) REFERENCES sales(id) ON DELETE CASCADE, -- If sale header is deleted, delete items
    CONSTRAINT FK_SaleItems_Mobile FOREIGN KEY (mobile_id) REFERENCES mobile(id) ON DELETE NO ACTION -- Don't allow deleting mobile if sold
);



CREATE TABLE sale_items (
    SaleItemID INT PRIMARY KEY IDENTITY(1,1),
    SaleID INT NOT NULL,             -- Foreign Key to the main sales header record
    mobile_id INT NOT NULL,          -- Foreign Key to the specific mobile sold (references mobile.mobile_id)
    QuantitySold INT NOT NULL,
    PriceAtSale DECIMAL(10, 2) NOT NULL, -- Price of ONE unit at the time of sale

    -- Optional: Calculated column for line total (Consider calculating in code instead if preferred)
    -- LineTotal AS (CAST(QuantitySold AS DECIMAL(10,2)) * PriceAtSale) PERSISTED,

    -- *** CORRECTED CONSTRAINT: References sales(SaleID) ***
    CONSTRAINT FK_SaleItems_Sales FOREIGN KEY (SaleID) REFERENCES sales(SaleID) ON DELETE CASCADE, -- If sale header is deleted, delete items

    -- *** CORRECTED CONSTRAINT: References mobile(mobile_id) ***
    CONSTRAINT FK_SaleItems_Mobile FOREIGN KEY (mobile_id) REFERENCES mobile(mobile_id) ON DELETE NO ACTION -- Don't allow deleting mobile if sold
);
GO -- Added GO statement for clarity if running in SSMS as a batch



ALTER TABLE dbo.sales
DROP COLUMN mobile_id;
GO

ALTER TABLE dbo.sales
DROP COLUMN quantity;
GO


USE master;
GO
SELECT database_id, name FROM sys.databases WHERE name = 'MobileShopManagementSystem'; -- Get the DB ID
GO
-- Replace 'Your_DB_ID' with the ID from the previous query
SELECT session_id, login_name, host_name, program_name
FROM sys.dm_exec_sessions
WHERE database_id = DB_ID('MobileShopManagementSystem');
GO

USE master;
GO
ALTER DATABASE MobileShopManagementSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO


DROP DATABASE MobileShopManagementSystem;
GO





