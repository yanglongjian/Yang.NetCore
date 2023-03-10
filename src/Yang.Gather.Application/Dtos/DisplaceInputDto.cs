using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yang.Gather.Application.Dtos
{
    public class DisplaceInputDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 原词汇
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// 新词汇
        /// </summary>
        public string NewWord { get; set; }
    }
}

