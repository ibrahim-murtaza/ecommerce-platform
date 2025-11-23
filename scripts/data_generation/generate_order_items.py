import csv
import random
from datetime import datetime, timedelta

def generate_order_items(num_items=400000, num_orders=500000, num_products=100000):
    # First, read orders to get valid OrderIDs and OrderDates for partitioning
    print("Reading orders file for date alignment...")
    order_dates = {}
    with open('orders.csv', 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        for idx, row in enumerate(reader, 1):
            order_dates[idx] = row['OrderDate']
    
    print(f"Loaded {len(order_dates)} order dates")
    
    with open('order_items.csv', 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['OrderID', 'OrderDate', 'ProductID', 'Quantity', 'PriceAtPurchase'])
        
        # Track items per order to ensure variety
        items_per_order = {}
        
        for i in range(1, num_items + 1):
            # Random order ID
            order_id = random.randint(1, num_orders)
            
            # Get the corresponding order date for partitioning
            order_date = order_dates.get(order_id, datetime.now().strftime('%Y-%m-%d %H:%M:%S'))
            
            # Random product
            product_id = random.randint(1, num_products)
            
            # Quantity between 1 and 10
            quantity = random.randint(1, 10)
            
            # Price at purchase (between $5 and $9999)
            price = round(random.uniform(5.00, 9999.99), 2)
            
            writer.writerow([order_id, order_date, product_id, quantity, price])
            
            if i % 50000 == 0:
                print(f"Generated {i} order items...")
        
        print(f"Completed! Generated {num_items} order items in order_items.csv")

if __name__ == "__main__":
    generate_order_items(400000)