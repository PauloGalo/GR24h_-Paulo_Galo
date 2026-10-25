using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using Microsoft.Data.SqlClient;

namespace GR24h_Repos
{
    public class RecipeIngredientRepository : IRepository <RecipeIngredient>
    {
        private readonly string _tableName = "[RecipeIngredient]";

        private RecipeIngredient Parse(SqlDataReader dataReader)
        {

            int id = Convert.ToInt32(dataReader["RecipeIngredient_id"]);
            int recipeId = Convert.ToInt32(dataReader["Recipe_id"]);
            int ingredientId = Convert.ToInt32(dataReader["Ingredient_id"]);

            decimal quantity = Convert.ToDecimal(dataReader["Quantity"]);
            string unitMeasure = Convert.ToString(dataReader["Unit_Measure"]) ?? string.Empty;
            

            return new RecipeIngredient(
                id,
                recipeId,
                ingredientId,
                quantity,
                unitMeasure

            );
        }

        public RecipeIngredient Create(RecipeIngredient entityRI)
        {
            string sql = $@"INSERT INTO {_tableName} (Recipe_id, Ingredient_id, Quantity, Unit_Measure) 
                            VALUES (@rId, @iId, @qty, @unit)";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@rId", entityRI.RecipeId);
            cmd.Parameters.AddWithValue("@iId", entityRI.IngredientId);
            cmd.Parameters.AddWithValue("@qty", entityRI.Quantity);
            cmd.Parameters.AddWithValue("@unit", entityRI.UnitMeasure);


            int newId = Sql.ExecuteNonQuery(cmd);
            return Retrieve(newId)!;
        }

        public RecipeIngredient? Retrieve(int id)
        {
            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $@" SELECT RecipeIngredient.RecipeIngredient_id, Recipe.Recipe_id,Recipe.Category_id,Recipe.Users_Id, 
                             Recipe.Recipe_Title, Recipe.Recipe_PreparationMethod, Recipe.Recipe_PreparationTime, Recipe.Recipe_Rating, 
                             Recipe.Recipe_ImageUrl, Recipe.Recipe_IsApproved, Ingredient.Ingredient_id, Ingredient.Ingredient_Name, 
                             RecipeIngredient.Quantity, RecipeIngredient.Unit_Measure
                             FROM {_tableName} RecipeIngredient
                             INNER JOIN [Recipe] Recipe ON RecipeIngredient.Recipe_id = Recipe.Recipe_id
                             INNER JOIN [Ingredient] Ingredient ON RecipeIngredient.Ingredient_id = Ingredient.Ingredient_id
                             WHERE RecipeIngredient.RecipeIngredient_id = @id";



                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = Sql.ExecuteQuery(cmd, conn))
                {
                    if (reader.Read())
                    {
                        return Parse(reader);
                    }
                    return null;
                }
            }
        }

        public List<RecipeIngredient> RetrieveAll()
        {
            List<RecipeIngredient> list = new List<RecipeIngredient>();

            using (SqlConnection conn = Sql.GetConnection())
            {
                string sql = $@" SELECT RecipeIngredient.RecipeIngredient_id, Recipe.Recipe_id,Recipe.Category_id,Recipe.Users_Id, 
                             Recipe.Recipe_Title, Recipe.Recipe_PreparationMethod, Recipe.Recipe_PreparationTime, Recipe.Recipe_Rating, 
                             Recipe.Recipe_ImageUrl, Recipe.Recipe_IsApproved, Ingredient.Ingredient_id, Ingredient.Ingredient_Name, 
                             RecipeIngredient.Quantity, RecipeIngredient.Unit_Measure
                             FROM {_tableName} RecipeIngredient
                             INNER JOIN [Recipe] Recipe ON RecipeIngredient.Recipe_id = Recipe.Recipe_id
                             INNER JOIN [Ingredient] Ingredient ON RecipeIngredient.Ingredient_id = Ingredient.Ingredient_id";


                SqlCommand cmd = new SqlCommand(sql);
                using (SqlDataReader reader = Sql.ExecuteQuery(cmd, conn))
                {
                    while (reader.Read())
                    {
                        list.Add(Parse(reader));
                    }
                    return list;
                }
            }

        }

        public void Update(RecipeIngredient entityRI)
        {
            string sql = $@"UPDATE {_tableName} SET 
                            Recipe_id = @rId, 
                            Ingredient_id = @iId, 
                            Quantity = @qty, 
                            Unit_Measure = @unit, 
                            WHERE RecipeIngredient_id = @id";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@rId", entityRI.RecipeId);
            cmd.Parameters.AddWithValue("@iId", entityRI.IngredientId);
            cmd.Parameters.AddWithValue("@qty", entityRI.Quantity);
            cmd.Parameters.AddWithValue("@unit", entityRI.UnitMeasure);
            cmd.Parameters.AddWithValue("@id", entityRI.RecipeIngredientId);
            Sql.ExecuteNonQuery(cmd);
        }

        public void Delete(int id)
        {
            string sql = $"DELETE FROM {_tableName} WHERE RecipeIngredient_id = @id";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@id", id);
            Sql.ExecuteNonQuery(cmd);
        }

    }
}
