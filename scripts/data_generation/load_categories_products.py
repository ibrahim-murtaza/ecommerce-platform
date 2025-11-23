import pyodbc
import csv

conn_str = (
    'DRIVER={ODBC Driver 17 for SQL Server};'
    'SERVER=localhost,1433;'
    'DATABASE=ECommerceDB;'
    'UID=sa;'
    'PWD=YourStrong!Passw0rd;'
)

try:
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()
    print("Connected to database successfully!")

    print("Loading categories...")
    with open('categories.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        for row in reader:
            cursor.execute(
                "INSERT INTO Category (CategoryName, Description, IsActive) VALUES (?, ?, ?)",
                row['CategoryName'], row['Description'], int(row['IsActive'])
            )
            count += 1
        conn.commit()
        print(f"Loaded {count} categories successfully!")

    print("Loading products...")
    with open('products.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                int(row['CategoryID']),
                row['ProductName'],
                row['Description'],
                float(row['Price']),
                int(row['StockQuantity']),
                row['ImageURL'],
                row['DateAdded'],
                int(row['IsActive'])
            ))

            if len(batch) >= 1000:
                cursor.executemany(
                    "INSERT INTO Product (CategoryID, ProductName, Description, Price, StockQuantity, ImageURL, DateAdded, IsActive) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                    batch
                )
                conn.commit()
                count += len(batch)
                print(f"Loaded {count} products...")
                batch = []

        if batch:
            cursor.executemany(
                "INSERT INTO Product (CategoryID, ProductName, Description, Price, StockQuantity, ImageURL, DateAdded, IsActive) VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                batch
            )
            conn.commit()
            count += len(batch)
        
        print(f"Loaded {count} products successfully!")
    
    cursor.close()
    conn.close()
    print("\nAll data loaded successfully!")
    
except Exception as e:
    print(f"Error: {e}")