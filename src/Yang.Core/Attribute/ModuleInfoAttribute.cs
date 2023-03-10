using System;

namespace Yang.Core
{

    /// <summary>
    /// 描述把当前功能(Controller或者Action)封装为一个模块(Module)节点，可以设置模块依赖的其他功能，模块的位置信息等
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ModuleInfoAttribute : Attribute
    {

        public ModuleInfoAttribute(string name, string postion = "")
        {
            Name = name;
            Position = postion;
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 模块代码
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int OrderCode { get; set; }
    }
}



