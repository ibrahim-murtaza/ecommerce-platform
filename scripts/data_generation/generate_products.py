import csv
import random
from datetime import datetime, timedelta

# Product name templates
product_templates = [
    "Premium",
    "Deluxe",
    "Classic",
    "Modern",
    "Vintage",
    "Professional",
    "Essential",
    "Ultimate",
    "Advanced",
    "Basic",
    "Pro",
    "Elite",
    "Standard",
    "Compact",
    "Portable",
]

product_types = [
    "Laptop",
    "Phone",
    "Tablet",
    "Watch",
    "Camera",
    "Headphones",
    "Speaker",
    "Monitor",
    "Keyboard",
    "Mouse",
    "Chair",
    "Desk",
    "Lamp",
    "Backpack",
    "Wallet",
    "Shoes",
    "Jacket",
    "Shirt",
    "Pants",
    "Hat",
    "Book",
    "Toy",
    "Game",
    "Tool",
    "Blender",
]


def generate_products(num_products=100000):
    with open("products.csv", "w", newline="", encoding="utf-8") as f:
        writer = csv.writer(f)
        writer.writerow(
            [
                "CategoryID",
                "ProductName",
                "Description",
                "Price",
                "StockQuantity",
                "ImageURL",
                "DateAdded",
                "IsActive",
            ]
        )

        for i in range(1, num_products + 1):
            category_id = random.randint(1, 50)

            # Generate product name
            template = random.choice(product_templates)
            product_type = random.choice(product_types)
            product_name = f"{template} {product_type} {i}"

            # Generate description
            description = f"High-quality {product_type.lower()} with excellent features and durability"

            # Generate realistic price (between $5 and $9999)
            price = round(random.uniform(5.00, 9999.99), 2)

            # Generate stock quantity (0-1000)
            stock = random.randint(0, 1000)

            # Generate image URL
            image_url = f"https://example.com/images/product_{i}.jpg"

            # Generate random date in the past year
            days_ago = random.randint(0, 365)
            date_added = (datetime.now() - timedelta(days=days_ago)).strftime(
                "%Y-%m-%d %H:%M:%S"
            )

            # 95% products are active
            is_active = 1 if random.random() < 0.95 else 0

            writer.writerow(
                [
                    category_id,
                    product_name,
                    description,
                    price,
                    stock,
                    image_url,
                    date_added,
                    is_active,
                ]
            )

            if i % 10000 == 0:
                print(f"Generated {i} products...")

    print(f"Completed! Generated {num_products} products in products.csv")


if __name__ == "__main__":
    generate_products(100000)
