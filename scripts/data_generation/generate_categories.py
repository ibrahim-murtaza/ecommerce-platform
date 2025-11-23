import csv
import random

categories = [
    "Electronics",
    "Clothing",
    "Books",
    "Home & Kitchen",
    "Sports & Outdoors",
    "Toys & Games",
    "Beauty & Personal Care",
    "Automotive",
    "Health & Wellness",
    "Jewelry",
    "Furniture",
    "Garden & Outdoor",
    "Pet Supplies",
    "Office Products",
    "Grocery & Gourmet",
    "Baby Products",
    "Tools & Home Improvement",
    "Arts & Crafts",
    "Musical Instruments",
    "Video Games",
    "Shoes",
    "Watches",
    "Luggage & Travel",
    "Industrial & Scientific",
    "Handmade Products",
    "Smart Home",
    "Camera & Photo",
    "Cell Phones & Accessories",
    "Computers & Tablets",
    "Audio & Headphones",
    "TV & Video",
    "Wearable Technology",
    "Kitchen Appliances",
    "Bedding & Bath",
    "Home Decor",
    "Lighting",
    "Storage & Organization",
    "Party Supplies",
    "Gift Cards",
    "Magazine Subscriptions",
    "Collectibles & Fine Art",
    "Entertainment Collectibles",
    "Sports Collectibles",
    "Trading Cards",
    "Coins & Paper Money",
    "Stamps",
    "Sewing & Needlework",
    "Scrapbooking",
    "Beading & Jewelry Making",
    "Painting & Drawing",
]

with open("categories.csv", "w", newline="", encoding="utf-8") as f:
    writer = csv.writer(f)
    writer.writerow(["CategoryName", "Description", "IsActive"])

    for i, category in enumerate(categories, 1):
        description = f"High-quality {category.lower()} products for all your needs"
        is_active = 1
        writer.writerow([category, description, is_active])

print(f"Generated {len(categories)} categories in categories.csv")
