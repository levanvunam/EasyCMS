using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Reflection.Enums;

namespace EzCMS.Core.Models.Tools.SessionManager
{
    public class SessionModel
    {
        public SessionModel()
        {
            IsComplexType = false;
        }

        public SessionModel(string name, object value)
            : this()
        {
            Id = name.ToIdString();
            Name = name;

            if (value == null)
            {
                Value = string.Empty;
            }
            else
            {
                var type = value.GetType();
                if (type.GetKind() == PropertyKind.Value)
                {
                    Value = value.ToString();
                }
                else
                {
                    Value = "Complex Session";
                    IsComplexType = true;
                }
            }
        }

        #region Public Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool IsComplexType { get; set; }

        #endregion
    }
}
