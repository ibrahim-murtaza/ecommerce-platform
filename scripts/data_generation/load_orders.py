import pyodbc
import csv

# Database connection
conn_str = (
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=localhost,1433;'
    'DATABASE=ECommerceDB;'
    'UID=sa;'
    'PWD=YourStrong!Passw0rd;'  # UPDATE WITH YOUR PASSWORD
)

try:
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()
    print("Connected to database successfully!")
    
    # Disable trigger that updates stock (to speed up bulk insert)
    print("Disabling stock update trigger...")
    cursor.execute("DISABLE TRIGGER trg_AfterOrderItem_UpdateStock ON OrderItem;")
    conn.commit()
    
    # Load Orders
    print("Loading orders...")
    with open('orders.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                int(row['UserID']),
                row['OrderDate'],
                float(row['TotalAmount']),
                row['Status'],
                row['ShippingAddress'],
                row['ShippingCity'],
                row['ShippingPostalCode']
            ))
            
            if len(batch) >= 5000:
                cursor.executemany(
                    "INSERT INTO [Order] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, ShippingCity, ShippingPostalCode) VALUES (?, ?, ?, ?, ?, ?, ?)",
                    batch
                )
                conn.commit()
                count += len(batch)
                print(f"Loaded {count} orders...")
                batch = []
        
        if batch:
            cursor.executemany(
                "INSERT INTO [Order] (UserID, OrderDate, TotalAmount, Status, ShippingAddress, ShippingCity, ShippingPostalCode) VALUES (?, ?, ?, ?, ?, ?, ?)",
                batch
            )
            conn.commit()
            count += len(batch)
        
        print(f"Loaded {count} orders successfully!")
    
    # Load Order Items
    print("Loading order items...")
    with open('order_items.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                int(row['OrderID']),
                row['OrderDate'],
                int(row['ProductID']),
                int(row['Quantity']),
                float(row['PriceAtPurchase'])
            ))
            
            if len(batch) >= 5000:
                cursor.executemany(
                    "INSERT INTO OrderItem (OrderID, OrderDate, ProductID, Quantity, PriceAtPurchase) VALUES (?, ?, ?, ?, ?)",
                    batch
                )
                conn.commit()
                count += len(batch)
                print(f"Loaded {count} order items...")
                batch = []
        
        if batch:
            cursor.executemany(
                "INSERT INTO OrderItem (OrderID, OrderDate, ProductID, Quantity, PriceAtPurchase) VALUES (?, ?, ?, ?, ?)",
                batch
            )
            conn.commit()
            count += len(batch)
        
        print(f"Loaded {count} order items successfully!")
    
    # Re-enable trigger
    print("Re-enabling stock update trigger...")
    cursor.execute("ENABLE TRIGGER trg_AfterOrderItem_UpdateStock ON OrderItem;")
    conn.commit()
    
    cursor.close()
    conn.close()
    print("\nAll order data loaded successfully!")
    
except Exception as e:
    print(f"Error: {e}")