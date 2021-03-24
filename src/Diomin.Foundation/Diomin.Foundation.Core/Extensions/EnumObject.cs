using System;

namespace Foundation.Core.Extensions
{
    /// <summary>
    /// 枚举、类型的值
    /// </summary>
    public struct EnumObject
    {
        public EnumObject(Enum um, string picture = null)
        {
            ID = (int)Convert.ChangeType(um, typeof(int));
            Name = um.ToString();
            Description = um.GetDescription();
            Picture = picture;
        }

        public EnumObject(int id, string name)
        {
            ID = id;
            Name = Description = name;
            Picture = null;
        }

        public EnumObject(int id, string name, string description, string picture)
        {
            ID = id;
            Name = name;
            Description = description;
            Picture = picture;
        }

        public int ID;

        public string Name;

        public string Description;

        public string Picture;
    }
    public struct EnumModel
    {
        public EnumModel(Enum um)
        {
            this.value = (int)Convert.ChangeType(um, typeof(int));
            this.name = um.ToString();
            this.text = um.GetDescription();
        }
        public int value { get; set; }
        public string name { get; set; }
        public string text { get; set; }
    }
}
