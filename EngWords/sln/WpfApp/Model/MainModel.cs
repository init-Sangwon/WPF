using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    internal class MainModel
    {
        public int Id { get; set; }
        public string Eng { get; set; }
        public string Kor { get; set; }

        public bool IsSelected { get; set; } = false; 
    }
}
