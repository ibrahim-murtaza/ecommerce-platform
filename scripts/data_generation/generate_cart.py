import csv
import random
from datetime import datetime, timedelta

def generate_cart_items(num_items=50000, num_users=10000, num_products=100000):
    with open('cart.csv', 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['UserID', 'ProductID', 'Quantity', 'DateAdded'])
        
        used_combinations = set()
        
        count = 0
        attempts = 0
        max_attempts = num_items * 2 
        
        while count < num_items and attempts < max_attempts:
            attempts += 1
            
            user_id = random.randint(1, num_users)
            product_id = random.randint(1, num_products)

            combo = (user_id, product_id)
            if combo in used_combinations:
                continue
            
            used_combinations.add(combo)

            quantity = random.randint(1, 5)

            days_ago = random.randint(0, 30)
            date_added = (datetime.now() - timedelta(days=days_ago)).strftime('%Y-%m-%d %H:%M:%S')
            
            writer.writerow([user_id, product_id, quantity, date_added])
            count += 1
            
            if count % 5000 == 0:
                print(f"Generated {count} cart items...")
        
        print(f"Completed! Generated {count} cart items in cart.csv")

if __name__ == "__main__":
    generate_cart_items(50000)