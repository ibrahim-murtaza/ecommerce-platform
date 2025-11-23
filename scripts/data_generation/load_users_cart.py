import pyodbc
import csv

# Database connection
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
    
    # Disable cart validation trigger for bulk insert
    print("Disabling cart validation trigger...")
    cursor.execute("DISABLE TRIGGER trg_InsteadOfCart_ValidateStock ON Cart;")
    conn.commit()
    
    # Load Users
    print("Loading users...")
    with open('users.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                row['Email'],
                row['PasswordHash'],
                row['FirstName'],
                row['LastName'],
                row['PhoneNumber'],
                row['Address'],
                row['City'],
                row['PostalCode'],
                row['DateJoined'],
                int(row['IsActive'])
            ))
            
            if len(batch) >= 1000:
                cursor.executemany(
                    "INSERT INTO [User] (Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, City, PostalCode, DateJoined, IsActive) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                    batch
                )
                conn.commit()
                count += len(batch)
                print(f"Loaded {count} users...")
                batch = []
        
        if batch:
            cursor.executemany(
                "INSERT INTO [User] (Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, City, PostalCode, DateJoined, IsActive) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                batch
            )
            conn.commit()
            count += len(batch)
        
        print(f"Loaded {count} users successfully!")
    
    # Load Admins
    print("Loading admins...")
    with open('admins.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                row['Email'],
                row['PasswordHash'],
                row['FirstName'],
                row['LastName'],
                row['Role'],
                row['DateCreated'],
                int(row['IsActive'])
            ))
        
        cursor.executemany(
            "INSERT INTO Admin (Email, PasswordHash, FirstName, LastName, Role, DateCreated, IsActive) VALUES (?, ?, ?, ?, ?, ?, ?)",
            batch
        )
        conn.commit()
        print(f"Loaded {len(batch)} admins successfully!")
    
    # Load Cart
    print("Loading cart items...")
    with open('cart.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        count = 0
        batch = []
        for row in reader:
            batch.append((
                int(row['UserID']),
                int(row['ProductID']),
                int(row['Quantity']),
                row['DateAdded']
            ))
            
            if len(batch) >= 1000:
                cursor.executemany(
                    "INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded) VALUES (?, ?, ?, ?)",
                    batch
                )
                conn.commit()
                count += len(batch)
                print(f"Loaded {count} cart items...")
                batch = []
        
        if batch:
            cursor.executemany(
                "INSERT INTO Cart (UserID, ProductID, Quantity, DateAdded) VALUES (?, ?, ?, ?)",
                batch
            )
            conn.commit()
            count += len(batch)
        
        print(f"Loaded {count} cart items successfully!")
        
    # Re-enable cart validation trigger
    print("Re-enabling cart validation trigger...")
    cursor.execute("ENABLE TRIGGER trg_InsteadOfCart_ValidateStock ON Cart;")
    conn.commit()
    
    cursor.close()
    conn.close()
    print("\nAll data loaded successfully!")
    
except Exception as e:
    print(f"Error: {e}")