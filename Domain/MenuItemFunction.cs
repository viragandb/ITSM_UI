using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class MenuItemFunction
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MenuItemFunctionId { get; set; }

        [Display(Name = "Menu Item")]
        public long MenuItemId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Menu Item Name")]
        public string MenuItemFunctionName { get; set; }

        [DefaultValue("")]
        [StringLength(200)]
        [Display(Name = "Area Name")]
        public string Area { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Controller Name")]
        public string Controller { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Action Name")]
        public string Action { get; set; }

        [MaxLength(200)]
        [Display(Name = "Menu Icon Name")]
        public string IconName { get; set; }

        public long MenuItemFunctionOrder { get; set; }

        [DefaultValue("False")]
        public bool IsDeleted { get; set; }


        #region Navigationl Properties
        public virtual MenuItem MenuItem { get; set; }
        public virtual ICollection<AccessControl> AccessControls { get; set; }
        #endregion

    }
}

