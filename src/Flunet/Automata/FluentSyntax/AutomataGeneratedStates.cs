namespace Flunet.Automata.FluentSyntax
{
#pragma warning disable 1591

    public partial class AutomataFluentSyntax
    {
        #region Nested type: IState0

        public interface IState0<T>
        {
            IState2<T> WithValidState(string name);

            IState2<T> WithInvalidState(string name);
        }

        #endregion

        #region Nested type: IState1

        public interface IState1
        {
        }

        #endregion

        #region Nested type: IState2

        public interface IState2<T>
        {
            IState2<T> WithValidState(string name);

            IState2<T> WithInvalidState(string name);

            IState3<T> IsRoot();

            IState4<T> TransitFrom(string name);

            IState4<T> TransitTo(string name);
        }

        #endregion

        #region Nested type: IState3

        public interface IState3<T>
        {
            IState3<T> WithValidState(string name);

            IState3<T> WithInvalidState(string name);

            IState5<T> TransitFrom(string name);

            IState5<T> TransitTo(string name);
        }

        #endregion

        #region Nested type: IState4

        public interface IState4<T>
        {
            IState2<T> OnInput(T input);
        }

        #endregion

        #region Nested type: IState5

        public interface IState5<T>
        {
            IState3<T> OnInput(T input);
        }

        #endregion

        #region Nested type: SyntaxImplementer

        internal abstract class SyntaxImplementer<T> : IState0<T>, IState1, IState2<T>, IState3<T>, IState4<T>,
                                                       IState5<T>
        {
            #region IState0<T> Members

            IState2<T> IState0<T>.WithValidState(string name)
            {
                return InnerWithValidState(name);
            }

            IState2<T> IState0<T>.WithInvalidState(string name)
            {
                return InnerWithInvalidState(name);
            }

            #endregion

            #region IState2<T> Members

            IState2<T> IState2<T>.WithValidState(string name)
            {
                return InnerWithValidState(name);
            }

            IState2<T> IState2<T>.WithInvalidState(string name)
            {
                return InnerWithInvalidState(name);
            }

            IState3<T> IState2<T>.IsRoot()
            {
                return InnerIsRoot();
            }

            IState4<T> IState2<T>.TransitFrom(string name)
            {
                return InnerTransitFrom(name);
            }

            IState4<T> IState2<T>.TransitTo(string name)
            {
                return InnerTransitTo(name);
            }

            #endregion

            #region IState3<T> Members

            IState3<T> IState3<T>.WithValidState(string name)
            {
                return InnerWithValidState(name);
            }

            IState3<T> IState3<T>.WithInvalidState(string name)
            {
                return InnerWithInvalidState(name);
            }

            IState5<T> IState3<T>.TransitFrom(string name)
            {
                return InnerTransitFrom(name);
            }

            IState5<T> IState3<T>.TransitTo(string name)
            {
                return InnerTransitTo(name);
            }

            #endregion

            #region IState4<T> Members

            IState2<T> IState4<T>.OnInput(T input)
            {
                return InnerOnInput(input);
            }

            #endregion

            #region IState5<T> Members

            IState3<T> IState5<T>.OnInput(T input)
            {
                return InnerOnInput(input);
            }

            #endregion

            protected abstract SyntaxImplementer<T> InnerWithValidState(string name);

            protected abstract SyntaxImplementer<T> InnerWithInvalidState(string name);

            protected abstract SyntaxImplementer<T> InnerIsRoot();

            protected abstract SyntaxImplementer<T> InnerTransitFrom(string name);

            protected abstract SyntaxImplementer<T> InnerTransitTo(string name);

            protected abstract SyntaxImplementer<T> InnerOnInput(T input);
        }

        #endregion

#pragma warning restore 1591
    }
}