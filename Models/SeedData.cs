using HealthWellbeing.Models;
using HealthWellbeing.Data;

internal class SeedData
{
	internal static void Populate(HealthWellbeingDbContext? dbContext)
	{
		if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

		// Ensure the database and tables are created.
		// Note: This is an alternative to migrations (Update-Database).
		// For production, using migrations is generally preferred.
		dbContext.Database.EnsureCreated();

		PopulateClients(dbContext);
        PopulateFoodCategory(dbContext);
        PopulateFoodComponent(dbContext);
    }

	private static void PopulateClients(HealthWellbeingDbContext dbContext)
	{
		// Check if the Client table already contains data
		if (dbContext.Client.Any()) return;

		dbContext.Client.AddRange(new List<Client>() {
			// Client 1: Fully Populated
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"), // Use a GUID for a unique ID
				Name = "Alice Wonderland",
				Email = "alice.w@example.com",
				Phone = "555-1234567",
				Address = "10 Downing St, London",
				BirthDate = new DateTime(1990, 5, 15),
				Gender = "Female",
				RegistrationDate = DateTime.Now.AddDays(-30), // Registered a month ago
				CreateMember = true
			},

			// Client 2: Minimal required fields + Pending Member
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"),
				Name = "Bob The Builder",
				Email = "bob.builder@work.net",
				Phone = "555-9876543",
				Address = "Construction Site 5A",
				BirthDate = new DateTime(1985, 10, 20),
				Gender = "Male",
				RegistrationDate = DateTime.Now.AddDays(-15),
				CreateMember = null // Pending Acceptation
			},

			// Client 3: Member Creation Rejected
			new Client
			{
				ClientId = Guid.NewGuid().ToString("N"),
				Name = "Charlie Brown",
				Email = "charlie.b@peanuts.com",
				Phone = "555-4567890",
				Address = "123 Comic Strip Ave",
				BirthDate = new DateTime(2000, 1, 1),
				Gender = "Male",
				RegistrationDate = DateTime.Now.AddDays(-5),
				CreateMember = false // Rejected/Denied
			}
		});
        
        dbContext.SaveChanges();
    }

    private static void PopulateFoodCategory(HealthWellbeingDbContext dbContext)
    {
        // Avoid duplicate seeding
        if (dbContext.FoodCategory.Any()) return;

        // --- Parent categories ---
        var fruits = new FoodCategory
        {
            Name = "Fruits",
            Description = "Natural foods rich in vitamins, fibers, and antioxidants."
        };

        var vegetables = new FoodCategory
        {
            Name = "Vegetables",
            Description = "Edible plant parts such as roots, stems, and leaves, essential for a balanced diet."
        };

        var proteins = new FoodCategory
        {
            Name = "Proteins",
            Description = "Animal or plant-based foods that provide amino acids for body repair and growth."
        };

        var grains = new FoodCategory
        {
            Name = "Grains & Cereals",
            Description = "Main sources of carbohydrates and energy, including whole grains and cereals."
        };

        var dairy = new FoodCategory
        {
            Name = "Dairy",
            Description = "Products derived from milk, rich in calcium, proteins, and vitamins."
        };

        var fats = new FoodCategory
        {
            Name = "Healthy Fats",
            Description = "Sources of good fats like omega-3, supporting heart and brain health."
        };

        // --- Subcategories (children) ---
        var citrus = new FoodCategory
        {
            Name = "Citrus Fruits",
            Description = "Fruits high in vitamin C, such as oranges and lemons.",
            ParentCategory = fruits
        };

        var tropical = new FoodCategory
        {
            Name = "Tropical Fruits",
            Description = "Exotic fruits rich in flavor and nutrients, like mangoes and pineapples.",
            ParentCategory = fruits
        };

        var leafy = new FoodCategory
        {
            Name = "Leafy Vegetables",
            Description = "Vegetables rich in iron and fiber, such as spinach and kale.",
            ParentCategory = vegetables
        };

        var root = new FoodCategory
        {
            Name = "Root Vegetables",
            Description = "Underground vegetables like carrots and sweet potatoes.",
            ParentCategory = vegetables
        };

        var meats = new FoodCategory
        {
            Name = "Meat & Poultry",
            Description = "High-protein foods such as chicken, beef, and turkey.",
            ParentCategory = proteins
        };

        var legumes = new FoodCategory
        {
            Name = "Legumes",
            Description = "Plant-based protein sources like beans, lentils, and chickpeas.",
            ParentCategory = proteins
        };

        var wholeGrains = new FoodCategory
        {
            Name = "Whole Grains",
            Description = "Grains that contain all essential parts of the grain seed, such as oats and brown rice.",
            ParentCategory = grains
        };

        var refinedGrains = new FoodCategory
        {
            Name = "Refined Grains",
            Description = "Processed grains like white rice and white bread.",
            ParentCategory = grains
        };

        var cheese = new FoodCategory
        {
            Name = "Cheese",
            Description = "Milk-derived products with varying textures and flavors.",
            ParentCategory = dairy
        };

        var yogurt = new FoodCategory
        {
            Name = "Yogurt",
            Description = "Fermented dairy products that support gut health.",
            ParentCategory = dairy
        };

        var nuts = new FoodCategory
        {
            Name = "Nuts & Seeds",
            Description = "Sources of healthy fats, vitamins, and proteins like almonds and chia seeds.",
            ParentCategory = fats
        };

        var oils = new FoodCategory
        {
            Name = "Vegetable Oils",
            Description = "Healthy oils for cooking, including olive and avocado oils.",
            ParentCategory = fats
        };

        // Add everything
        dbContext.FoodCategory.AddRange(new[]
        {
        fruits, vegetables, proteins, grains, dairy, fats,
        citrus, tropical, leafy, root, meats, legumes,
        wholeGrains, refinedGrains, cheese, yogurt, nuts, oils
    });

        dbContext.SaveChanges();
    }


    private static void PopulateFoodComponent(HealthWellbeingDbContext dbContext)
        {
        // Check if the FoodComponent table already contains data
        if (dbContext.FoodComponent.Any()) return;

        dbContext.FoodComponent.AddRange(new List<FoodComponent>()
        {
            new FoodComponent
            {
                Name = "Apple",
                Description = "Contains water, carbohydrates, fibers, vitamins, minerals, and antioxidants."
            },
            new FoodComponent
            {
                Name = "Rice",
                Description = "Rich in carbohydrates and provides a quick source of energy."
            },
            new FoodComponent
            {

                Name = "Salmon",
                Description = "Contains omega-3 fatty acids, proteins, and essential minerals."
            },
            new FoodComponent
            {
                Name = "Broccoli",
                Description = "Contains fiber, vitamins C and K, iron, and potassium."
            },
            new FoodComponent
            {
                Name = "Almonds",
                Description = "Rich in healthy fats, proteins, and vitamin E."
            },
            new FoodComponent
            {
                Name = "Eggs",
                Description = "Excellent source of protein and essential amino acids."
            },
            new FoodComponent
            {
                Name = "Oats",
                Description = "Contain fiber, vitamins, and minerals that help reduce cholesterol."
            },
            new FoodComponent
            {
                Name = "Tomato",
                Description = "Contains lycopene, an antioxidant that helps prevent heart disease."
            },
            new FoodComponent
            {
                Name = "Spinach",
                Description = "Rich in iron, calcium, magnesium, and vitamin A."
            },
            new FoodComponent
            {
                Name = "Banana",
                Description = "Good source of potassium, vitamin B6, and natural sugars for energy."
            }
        });

		dbContext.SaveChanges();
	}
}