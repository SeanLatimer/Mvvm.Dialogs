﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using HanumanInstitute.MvvmDialogs;

namespace Demo.Avalonia.NonModalCustomDialog;

public class CurrentTimeCustomDialog : IView
{
    private readonly CurrentTimeDialog _dialog = new();

    public void Initialize(INotifyPropertyChanged viewModel, Type viewType)
    {
        ViewModel = viewModel;
    }
    public void InitializeExisting(INotifyPropertyChanged viewModel, object view)
    {
        ViewModel = viewModel;
    }

    public Type ViewType { get; set; } = default!;

    public object RefObj => this;

    public event EventHandler Loaded
    {
        add => _dialog.Opened += value;
        remove => _dialog.Opened -= value;
    }

    public event EventHandler Closed
    {
        add => _dialog.Closed += value;
        remove => _dialog.Closed -= value;
    }

    public event EventHandler<CancelEventArgs>? Closing
    {
        add
        {
            if (value != null)
            {
                var handler = new EventHandler<WindowClosingEventArgs>(value.Invoke);
                _closingHandlers.Add(value, handler);
                _dialog.Closing += handler;
            }
        }
        remove
        {
            if (value != null)
            {
                _dialog.Closing += _closingHandlers[value];
                _closingHandlers.Remove(value);
            }
        }
    }
    private readonly Dictionary<EventHandler<CancelEventArgs>, EventHandler<WindowClosingEventArgs>> _closingHandlers = new();

    public INotifyPropertyChanged ViewModel
    {
        get => (INotifyPropertyChanged)_dialog.DataContext!;
        set => _dialog.DataContext = value;
    }

    public Task ShowDialogAsync(IView owner)
    {
        return _dialog.ShowDialog<bool?>((Window)owner.RefObj);
    }

    public void Show(IView? owner) => _dialog.Show((Window)owner!.RefObj);

    public void Activate() => _dialog.Activate();

    public void Close() => _dialog.Close();

    public bool IsEnabled
    {
        get => _dialog.IsEnabled;
        set => _dialog.IsEnabled = value;
    }

    public bool IsVisible => _dialog.IsEnabled;

    public bool ClosingConfirmed { get; set; }
}
