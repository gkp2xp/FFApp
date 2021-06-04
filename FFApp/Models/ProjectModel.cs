using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFApp.Models
{
    public class ProjectModel
    {
        /// <summary>
        /// Investment Unique Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Number of Days of the projected investment. Must be positif
        /// </summary>
        public int days { get; set; }
        /// <summary>
        /// Expected Annual Return expressed as an integer
        /// 12% -> 120, 5% -> 5
        /// </summary>
        public int expectedReturn { get; set; }
    }
}
