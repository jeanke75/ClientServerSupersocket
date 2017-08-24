using System.Configuration;

namespace SocketServer.Config
{
    public class AccountConfig : ConfigurationSection
    {
        private AccountConfig() { }

        [ConfigurationProperty("minPasswordLength", DefaultValue = 6, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 10)]
        public int MinPasswordLength
        {
            get
            {
                return (int)this["minPasswordLength"];
            }
            set
            {
                this["minPasswordLength"] = value;
            }
        }

        [ConfigurationProperty("register")]
        public RegisterConfigElement Register
        {
            get
            {
                return (RegisterConfigElement)this["register"];
            }
        }

        /*[ConfigurationProperty("login")]
        public LoginConfigElement LoginSection
        {
            get
            {
                return (LoginConfigElement)this["login"];
            }
        }*/
    }

    public class RegisterConfigElement : ConfigurationElement
    {
        private RegisterConfigElement() { }

        [ConfigurationProperty("defaultMap", DefaultValue = "TestMap", IsRequired = true)]
        [StringValidator(InvalidCharacters = "_~!?@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string MapName
        {
            get
            {
                return (string)this["defaultMap"];
            }
            set
            {
                this["defaultMap"] = value;
            }
        }

        [ConfigurationProperty("startPosX", DefaultValue = 0, IsRequired = true)]
        [IntegerValidator(MinValue = ushort.MinValue, MaxValue = ushort.MaxValue)]
        public int X
        {
            get
            {
                return (int)this["startPosX"];
            }
            set
            {
                this["startPosX"] = value;
            }
        }

        [ConfigurationProperty("startPosY", DefaultValue = 0, IsRequired = true)]
        [IntegerValidator(MinValue = ushort.MinValue, MaxValue = ushort.MaxValue)]
        public int Y
        {
            get
            {
                return (int)this["startPosY"];
            }
            set
            {
                this["startPosY"] = value;
            }
        }
    }

    /*public class LoginConfigElement : ConfigurationElement
    {
        
    }*/
}
