import csv
import random
from datetime import datetime, timedelta

statuses = ['Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled']
status_weights = [0.05, 0.10, 0.15, 0.65, 0.05]  # Most orders are delivered

cities = [
    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
    "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte", "Seattle",
    "Denver", "Boston", "Portland", "Las Vegas", "Detroit", "Memphis", "Nashville", "Baltimore", "Milwaukee"
]

street_names = ["Main St", "Oak Ave", "Maple Dr", "Cedar Ln", "Elm St", "Park Ave", "Washington Blvd", "Lincoln Way"]

def generate_orders(num_orders=500000, num_users=10000):
    with open('orders.csv', 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['UserID', 'OrderDate', 'TotalAmount', 'Status', 'ShippingAddress', 'ShippingCity', 'ShippingPostalCode'])
        
        # Generate orders over the past 2 years
        start_date = datetime.now() - timedelta(days=730)
        
        for i in range(1, num_orders + 1):
            # Random user (some users order multiple times)
            user_id = random.randint(1, num_users)
            
            # Random date in the past 2 years
            days_offset = random.randint(0, 730)
            order_date = (start_date + timedelta(days=days_offset)).strftime('%Y-%m-%d %H:%M:%S')
            
            # Random total amount between $10 and $5000
            total_amount = round(random.uniform(10.00, 5000.00), 2)
            
            # Status based on weights
            status = random.choices(statuses, weights=status_weights)[0]
            
            # Shipping address
            street_num = random.randint(1, 9999)
            shipping_address = f"{street_num} {random.choice(street_names)}"
            shipping_city = random.choice(cities)
            shipping_postal = f"{random.randint(10000, 99999)}"
            
            writer.writerow([user_id, order_date, total_amount, status, shipping_address, shipping_city, shipping_postal])
            
            if i % 50000 == 0:
                print(f"Generated {i} orders...")
    
    print(f"Completed! Generated {num_orders} orders in orders.csv")

if __name__ == "__main__":
    generate_orders(500000)