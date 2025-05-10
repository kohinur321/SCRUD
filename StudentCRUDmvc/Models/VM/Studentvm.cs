using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentCRUDmvc.Models.VM
{
    public class Studentvm
    {
        public Studentvm()
        {
            this.Admissions = new HashSet<Admission>();
        }

        public int Stu_id { get; set; }
        public string Stu_name { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<bool> MorningShift { get; set; }
        public string Picture { get; set; }
        public HttpPostedFileBase PictureFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage",
        "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Admission> Admissions { get; set; }
    }
}