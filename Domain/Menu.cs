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
    public class Menu
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MenuId { get; set; }

        public long ModuleId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

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

        public long MenuOrder { get; set; }

        [DefaultValue("False")]
        public bool IsDeleted { get; set; }




        #region Navigational Properties

        public virtual ICollection<MenuItem> MenuItems { get; set; }

        #endregion
    }
}

