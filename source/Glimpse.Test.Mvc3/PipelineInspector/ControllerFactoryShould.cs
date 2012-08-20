﻿using System.Collections.Generic;
using System.Web.Mvc;
using Glimpse.Core.Extensibility;
using Glimpse.Mvc3.PipelineInspector;
using Moq;
using Xunit;

namespace Glimpse.Test.Mvc3.PipelineInspector
{
    public class ControllerFactoryShould
    {
        [Fact]
        public void AbortIfUnableToProxy()
        {
            ControllerBuilder.Current.SetControllerFactory(typeof(DefaultControllerFactory));


            var factoryMock = new Mock<IProxyFactory>();
            factoryMock.Setup(f => f.IsProxyable(It.IsAny<object>())).Returns(false);


            var contextMock = new Mock<IPipelineInspectorContext>();
            contextMock.Setup(c => c.ProxyFactory).Returns(factoryMock.Object);

            var inspector = new ControllerFactory();

            inspector.Setup(contextMock.Object);
            
            contextMock.Verify(c=>c.Logger, Times.Never());
        }

        [Fact]
        public void ProxyControllerFactory()
        {
            ControllerBuilder.Current.SetControllerFactory(typeof(DefaultControllerFactory));

            var controllerFactoryMock = new Mock<IControllerFactory>();


            var loggerMock = new Mock<ILogger>();

            var factoryMock = new Mock<IProxyFactory>();
            factoryMock.Setup(f => f.IsProxyable(It.IsAny<object>())).Returns(true);
            factoryMock.Setup(
    f =>
    f.CreateProxy(It.IsAny<IControllerFactory>(),
                  It.IsAny<IEnumerable<IAlternateImplementation<IControllerFactory>>>())).Returns(
                      controllerFactoryMock.Object);

            var contextMock = new Mock<IPipelineInspectorContext>();
            contextMock.Setup(c => c.ProxyFactory).Returns(factoryMock.Object);
            contextMock.Setup(c => c.Logger).Returns(loggerMock.Object);

            var inspector = new ControllerFactory();

            inspector.Setup(contextMock.Object);

            Assert.Equal(ControllerBuilder.Current.GetControllerFactory(), controllerFactoryMock.Object);
            loggerMock.Verify(l=>l.Debug(It.IsAny<string>(), It.IsAny<object[]>()));
        }
    }
}