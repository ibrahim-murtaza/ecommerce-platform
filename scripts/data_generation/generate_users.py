import csv
import random
from datetime import datetime, timedelta

# Sample data for realistic names
first_names = [
    "John", "Jane", "Michael", "Sarah", "David", "Emily", "Chris", "Lisa", "Daniel", "Ashley",
    "James", "Jessica", "Robert", "Amanda", "William", "Melissa", "Richard", "Jennifer", "Joseph", "Laura",
    "Thomas", "Stephanie", "Charles", "Rebecca", "Matthew", "Nicole", "Andrew", "Rachel", "Joshua", "Elizabeth"
]

last_names = [
    "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
    "Wilson", "Anderson", "Taylor", "Thomas", "Moore", "Jackson", "Martin", "Lee", "Thompson", "White",
    "Harris", "Clark", "Lewis", "Robinson", "Walker", "Young", "Allen", "King", "Wright", "Scott"
]

cities = [
    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
    "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte", "Seattle",
    "Denver", "Boston", "Portland", "Las Vegas", "Detroit", "Memphis", "Nashville", "Baltimore", "Milwaukee"
]

def generate_users(num_users=10000):
    with open('users.csv', 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['Email', 'PasswordHash', 'FirstName', 'LastName', 'PhoneNumber', 
                        'Address', 'City', 'PostalCode', 'DateJoined', 'IsActive'])
        
        for i in range(1, num_users + 1):
            first_name = random.choice(first_names)
            last_name = random.choice(last_names)
            email = f"{first_name.lower()}.{last_name.lower()}{i}@email.com"
            
            # Simple password hash (in production, use proper hashing)
            password_hash = f"hash_{random.randint(100000, 999999)}"
            
            # Generate phone number
            phone = f"+1-{random.randint(200, 999)}-{random.randint(100, 999)}-{random.randint(1000, 9999)}"
            
            # Generate address
            street_num = random.randint(1, 9999)
            street_names = ["Main St", "Oak Ave", "Maple Dr", "Cedar Ln", "Elm St", "Park Ave"]
            address = f"{street_num} {random.choice(street_names)}"
            
            city = random.choice(cities)
            postal_code = f"{random.randint(10000, 99999)}"
            
            # Random date in the past 2 years
            days_ago = random.randint(0, 730)
            date_joined = (datetime.now() - timedelta(days=days_ago)).strftime('%Y-%m-%d %H:%M:%S')
            
            # 98% users are active
            is_active = 1 if random.random() < 0.98 else 0
            
            writer.writerow([email, password_hash, first_name, last_name, phone, 
                           address, city, postal_code, date_joined, is_active])
            
            if i % 1000 == 0:
                print(f"Generated {i} users...")
    
    print(f"Completed! Generated {num_users} users in users.csv")

if __name__ == "__main__":
    generate_users(10000)