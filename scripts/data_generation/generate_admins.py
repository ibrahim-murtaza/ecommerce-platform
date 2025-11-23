import csv
import random
from datetime import datetime, timedelta

admin_first_names = [
    "Admin", "Manager", "Supervisor", "Director", "Chief", "Head", "Lead", "Senior", "Principal", "Executive"
]

admin_last_names = [
    "System", "Operations", "Sales", "Marketing", "Finance", "Support", "Product", "Technology", "Security", "Analytics"
]

roles = ["Admin", "SuperAdmin", "Manager", "Moderator"]

def generate_admins(num_admins=100):
    with open('admins.csv', 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['Email', 'PasswordHash', 'FirstName', 'LastName', 'Role', 'DateCreated', 'IsActive'])
        
        for i in range(1, num_admins + 1):
            first_name = random.choice(admin_first_names)
            last_name = random.choice(admin_last_names)
            email = f"admin{i}@ecommerce.com"
            
            # Admin password hash
            password_hash = f"admin_hash_{random.randint(100000, 999999)}"
            
            role = random.choice(roles)
            
            # Random date in the past year
            days_ago = random.randint(0, 365)
            date_created = (datetime.now() - timedelta(days=days_ago)).strftime('%Y-%m-%d %H:%M:%S')
            
            # All admins are active
            is_active = 1
            
            writer.writerow([email, password_hash, first_name, last_name, role, date_created, is_active])
            
            if i % 20 == 0:
                print(f"Generated {i} admins...")
    
    print(f"Completed! Generated {num_admins} admins in admins.csv")

if __name__ == "__main__":
    generate_admins(100)