using EasyCQRS.Azure.Config;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EasyCQRS.Azure.Tests
{
    public class DefaultConfigurationManagerTests
    {
        [Fact]
        public void DefaultConfigurationManager_IsAssignableFromIConfigurationManager()
        {
            var sut = new DefaultConfigurationManager(Mock.Of<Microsoft.Extensions.Configuration.IConfigurationRoot>());

            Assert.IsAssignableFrom<IConfigurationManager>(sut);
        }

        [Fact]
        public void DefaultConfigurationManager_GetSettingUseConfigurationRootIndexer()
        {
            var settingName = "Fake Setting";
            var mock = new Mock<Microsoft.Extensions.Configuration.IConfigurationRoot>();

            var sut = new DefaultConfigurationManager(mock.Object);

            mock.Setup(m => m[settingName]).Returns(string.Empty);

            sut.GetSetting(settingName);


            mock.Verify(m => m[settingName]);
        }

        [Fact]
        public void DefaultConfigurationManager_GetSettingTranslateDottes()
        {
            var settingName = "Fake.Dot.Separed.Setting";
            var realSettingName = "Fake:Dot:Separed:Setting";

            var mock = new Mock<Microsoft.Extensions.Configuration.IConfigurationRoot>();

            var sut = new DefaultConfigurationManager(mock.Object);

            mock.Setup(m => m[realSettingName]).Returns(string.Empty);

            sut.GetSetting(settingName);

            mock.Verify(m => m[realSettingName]);
        }
    }
}
