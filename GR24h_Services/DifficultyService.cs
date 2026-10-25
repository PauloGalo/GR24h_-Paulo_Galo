using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR24h_Models;
using GR24h_Repos;

namespace GR24h_Services
{
    public class DifficultyService
    {
        private readonly DifficultyRepository _difficultyRepo;
        private readonly RecipeService _recipeService;

        public DifficultyService(DifficultyRepository difficultyRepo, RecipeService recipeService)
        {
            _difficultyRepo = difficultyRepo;
            _recipeService = recipeService;
        }


        public void CreateDifficultyByAdmin(Difficulty newDifficulty)
        {
            bool alreadyExists = _difficultyRepo.RetrieveAll()
                .Any(d => d.Name.ToLower() == newDifficulty.Name.ToLower());

            if (alreadyExists)
                throw new Exception($"Nível de Dificuldade '{newDifficulty.Name}'já existe.Tente novamente.");

            _difficultyRepo.Create(newDifficulty);
        }


        public Difficulty GetDifficultyById(int diffId)
        {
            if (diffId <= 0)
                throw new Exception("Operação Falhada : ID de Nível de Dificuldade inválido.");

            Difficulty? difficulty = _difficultyRepo.Retrieve(diffId);

            if (difficulty == null)
                throw new Exception("Operação Falhada : Nível de Dificuldade solicitado não existe.");


            return difficulty;
        }


        public List<Difficulty> GetAllDifficulties()
        {
            List<Difficulty> difficulties = _difficultyRepo.RetrieveAll();

            if (difficulties == null)
            {
                return new List<Difficulty>();
            }

            return difficulties
                .OrderBy(d => d.Name)
                .ToList();
        }


        public void UpdateDifficultyByAdmin(Difficulty updatedDifficulty)
        {
            if (updatedDifficulty.Id <= 0)
                throw new Exception("Operação Falhada : ID de Dificuldade inválido.");

            bool alreadyExists = _difficultyRepo.RetrieveAll()
                .Any(d => d.Name.ToLower() == updatedDifficulty.Name.ToLower() && d.Id != updatedDifficulty.Id);

            if (alreadyExists)
                throw new Exception($"Nível de dificuldade '{updatedDifficulty.Name}' já existe.Tente novamente.");

            _difficultyRepo.Update(updatedDifficulty);
        }



        public void DeleteDifficultyByAdmin(int id)
        {
            if (id <= 0)
                throw new Exception("Operação Falhada : ID de Dificuldade inválido.");

            bool hasRecipes = _recipeService.GetAllRecipesDetails().Any(r => r.Recipe.DifficultyId == id);

            if (hasRecipes)
                throw new Exception("Operação Negada: Existem receitas associadas a este nível de dificuldade.");

            _difficultyRepo.Delete(id);
        }
    }
}
