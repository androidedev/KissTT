﻿using System.ComponentModel;
using System.Runtime.CompilerServices;


// this implementation is from Microsoft Prism lib and modified

namespace ktt3
{

    /// <summary>
    /// BindableBase base class from which you can derive your model / view model classes that implements the INotifyPropertyChanged interface in a type-safe manner,
    /// as shown here :
    /// 
    /// public TransactionInfo TransactionInfo
    /// {
    ///    get { return this.transactionInfo; }
    ///    set 
    ///    { 
    ///         SetProperty(ref this.transactionInfo, value);
    ///         this.OnPropertyChanged(() => this.TickerSymbol);
    ///    }
    /// }
    /// </summary>
    /// 

    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            //var eventHandler = this.PropertyChanged;
            //if (eventHandler != null)
            //{
            //    eventHandler(this, new PropertyChangedEventArgs(propertyName));
            //}
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///// <summary>
        ///// Raises this object's PropertyChanged event.
        ///// </summary>
        ///// <typeparam name="T">The type of the property that has a new value</typeparam>
        ///// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        //protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        //{
        //    var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
        //    this.OnPropertyChanged(propertyName);
        //}

    }

}
