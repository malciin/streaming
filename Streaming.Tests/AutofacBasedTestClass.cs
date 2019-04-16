using Autofac;
using NUnit.Framework;
using System;

namespace Streaming.Tests
{
    public abstract class AutofacBasedTestClass
    {
        private Action<ContainerBuilder> perTestRegisterMethod = null;

        protected Action<ContainerBuilder> PerTestRegisterMethod
        {
            get
            {
                return perTestRegisterMethod;
            }
            set
            {
                if (ctx != null)
                {
                    throw new FieldAccessException(
                        "Ensure that PerTestRegisterMethod is setted before " +
                        "any this.Context call");
                }
                perTestRegisterMethod = value;
            }
        }

        private ContainerBuilder builder = null;
        private IComponentContext ctx;
        protected IComponentContext Context
        {
            get
            {
                if (ctx == null)
                {
                    if (perTestRegisterMethod != null)
                        PerTestRegisterMethod(builder);
                    ctx = builder.Build();
                }
                return ctx;
            }
        }

        [SetUp]
        public void Setup()
        {
            perTestRegisterMethod = null;
            ctx = null;
            builder = new ContainerBuilder();
            SetupRegister(builder);
        }

        protected abstract void SetupRegister(ContainerBuilder builder);
    }
}
