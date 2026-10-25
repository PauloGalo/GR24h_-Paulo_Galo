using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Models
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;

        public Category(int id, string valueCategoryName)
        {
            this.Id = id;
            this.Name = ValidateName(valueCategoryName);
        }

        private string ValidateName(string valueCategoryName)
        {
            if (string.IsNullOrWhiteSpace(valueCategoryName))
                throw new ArgumentException("Campo obrigatório");

            valueCategoryName = valueCategoryName.Trim();


            if (valueCategoryName.Length < 5 || valueCategoryName.Length > 25)
                throw new ArgumentException("Categoria deve conter entre 5 a 25 caracteres.Tente novamente.");

            if (!valueCategoryName.All(c => char.IsLetter(c)|| char.IsWhiteSpace(c)))
                throw new ArgumentException("Categoria deve conter apenas letras e espaços.Tente novamente.");

            return valueCategoryName;
        }
    }
}
