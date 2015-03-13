using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Isidos.Paysera
{
    public class EntityValidationResult
    {
        public IList<ValidationResult> Errors { get; private set; }

        public bool HasError
        {
            get { return Errors.Any(); }
        }

        public EntityValidationResult(IList<ValidationResult> errors = null)
        {
            Errors = errors ?? new List<ValidationResult>();
        }
    }
}