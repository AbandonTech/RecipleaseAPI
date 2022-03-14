# Reciplease API


Dummy/Test SQL for database
```sql
CREATE TABLE recipe (
  id INT PRIMARY KEY,
  servings INT NOT NULL
);

CREATE TABLE ingredient (
	name TEXT UNIQUE PRIMARY KEY
);

INSERT INTO ingredient (name) VALUES
  ('Chicken'),
  ('Mushroom'),
  ('Orange');

CREATE TABLE recipe_ingredient (
  recipe_id INT,
  ingredient TEXT REFERENCES ingredient (name) ON UPDATE CASCADE,
  
  CONSTRAINT fk_recipe
    FOREIGN KEY(recipe_id) 
	  REFERENCES recipe(id)
);


INSERT INTO recipe (id, servings) VALUES (1, 1);
INSERT INTO recipe (id, servings) VALUES (2, 1);
INSERT INTO recipe (id, servings) VALUES (3, 1);


INSERT INTO recipe_ingredient (recipe_id, ingredient) VALUES (1, 'Chicken');
INSERT INTO recipe_ingredient (recipe_id, ingredient) VALUES (1, 'Mushroom');
INSERT INTO recipe_ingredient (recipe_id, ingredient) VALUES (2, 'Mushroom');
INSERT INTO recipe_ingredient (recipe_id, ingredient) VALUES (3, 'Orange');
```