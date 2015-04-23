////////////////////////////////////////////////////////////////////////////
// <copyright file="AutomationEventManager.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// This is a wrapper class for the .NET UI Automation API.
    /// It expoeses functions to add/remove automation event handlers
    /// and property changed event handlers.  It keeps track of the
    /// handlers that have been added to windows/controls so if
    /// the app adds new handlers to the same window, they are
    /// not re-added. Also, if a window is closed, it automatically
    /// removes all the handlers associcated with the window.
    /// A key requirement for UI Automation is that the handlers should
    /// be removed from the same thread on which they were added.  This
    /// class takes care of this by creating a thread and adding and
    /// removing handlers from the same thread.  This gives the app
    /// the flexibility to add/remove handlers from any thread.
    /// </summary>
    public class AutomationEventManager : IDisposable
    {
        /// <summary>
        /// Returns the singleton instance
        /// </summary>
        private static readonly AutomationEventManager _instance = new AutomationEventManager();

        /// <summary>
        /// Maps a window handle to its WindowElement object (see below)
        /// </summary>
        static private readonly Hashtable WindowTable = new Hashtable();

        /// <summary>
        /// Queue on which add/remove requests are added. They are
        /// processed asnchronously on a separate thread.
        ///
        /// </summary>
        private readonly BlockingQueue<object> _queue = new BlockingQueue<object>();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// To let the thread know that we are done on disposal
        /// </summary>
        private volatile bool _done;

        /// <summary>
        /// Thread on which handlers are added/removed
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Prevents a default instance of AutomationEventManager class from being created
        /// </summary>
        private AutomationEventManager()
        {
            Start();
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static AutomationEventManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Adds an automation event handler for the specified element in the
        /// specified window
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="autoEvent">the event to add</param>
        /// <param name="element">the automation element for the control</param>
        /// <param name="eventHandler">the event handler for the event</param>
        static public void AddAutomationEventHandler(IntPtr hWnd,
                                        AutomationEvent autoEvent,
                                        AutomationElement element,
                                        AutomationEventHandler eventHandler)
        {
            Log.Debug();

            var windowElement = (WindowElement)WindowTable[hWnd];
            if (windowElement == null)
            {
                windowElement = new WindowElement(hWnd);
                WindowTable.Add(hWnd, windowElement);

                var control = Form.FromHandle(hWnd);
                if (control is Form)
                {
                    var form = control as Form;
                    form.VisibleChanged += form_VisibleChanged;
                    form.FormClosing += form_FormClosing;
                }
                else
                {
                    windowElement.EvtOnWindowClosed += windowElement_EvtOnWindowClosed;
                }
            }
            else
            {
                Log.Debug("Found window element");
            }

            // create the item and add it
            var item = new AddEventHandlerItem
            {
                AutoEvent = autoEvent,
                AutoElement = element,
                EventHandler = eventHandler,
                WinElement = windowElement
            };

            AddAutomationEvent(item);
        }

        /// <summary>
        /// Adds a property changed automation event handler
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="property">the property to track</param>
        /// <param name="element">the element in the window to track</param>
        /// <param name="eventHandler">the event handler</param>
        static public void AddAutomationPropertyChangedEventHandler(IntPtr hWnd,
                                                            AutomationProperty property,
                                                            AutomationElement element,
                                                            AutomationPropertyChangedEventHandler eventHandler)
        {
            lock (WindowTable)
            {
                var windowElement = (WindowElement)WindowTable[hWnd];
                if (windowElement == null)
                {
                    windowElement = new WindowElement(hWnd);
                    WindowTable.Add(hWnd, windowElement);
                    windowElement.EvtOnWindowClosed += windowElement_EvtOnWindowClosed;
                }
                var item = new AddAutomationPropertyChangedItem
                {
                    Property = property,
                    AutoElement = element,
                    EventHandler = eventHandler,
                    WinElement = windowElement
                };

                AddAutomationPropertyChanged(item);
            }
        }

        /// <summary>
        /// Removes all the automation event handlers for the
        /// specified window
        /// </summary>
        /// <param name="hwnd">window handle</param>
        static public void RemoveAllAutomationEventHandlers(IntPtr hwnd)
        {
            Log.Debug(hwnd.ToString());

            lock (WindowTable)
            {
                if (WindowTable.Contains(hwnd))
                {
                    var windowElement = (WindowElement)WindowTable[hwnd];

                    windowElement.EvtOnWindowClosed -= windowElement_EvtOnWindowClosed;
                    Log.Debug("Found " + hwnd + " in hashtable. removing it");
                    WindowTable.Remove(hwnd);

                    Log.Debug("Calling RemoveAllEvents");

                    var item = new RemoveAllEventsItem { WinElement = windowElement };
                    RemoveAllEvents(item);
                }
                else
                {
                    Log.Debug("Did not find " + hwnd + " in hashtable.");
                }
            }
        }

        /// <summary>
        /// Removes event handler for the specified window and element
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="autoEvent">the event to remove</param>
        /// <param name="element">the automation element</param>
        static public void RemoveAutomationEventHandler(IntPtr hWnd, AutomationEvent autoEvent, AutomationElement element)
        {
            Log.Debug("hWnd=" + hWnd);
            if (hWnd != IntPtr.Zero)
            {
                lock (WindowTable)
                {
                    var windowElement = (WindowElement)WindowTable[hWnd];
                    if (windowElement != null)
                    {
                        var item = new RemoveEventHandlerItem
                        {
                            AutoEvent = autoEvent,
                            AutoElement = element,
                            WinElement = windowElement
                        };
                        RemoveAutomationEvent(item);

                        //windowElement.RemoveAutomationEventHandler(element, autoEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a previously added property changed event handler
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="property">the property to remove</param>
        /// <param name="element">the control in the window</param>
        /// <param name="eventHandler">the eveht handler to remove</param>
        static public void RemoveAutomationPropertyChangedEventHandler(IntPtr hWnd,
                                                                AutomationProperty property,
                                                                AutomationElement element,
                                                                AutomationPropertyChangedEventHandler eventHandler)
        {
            Log.Debug();

            if (hWnd == IntPtr.Zero)
            {
                return;
            }

            lock (WindowTable)
            {
                var windowElement = (WindowElement)WindowTable[hWnd];
                if (windowElement != null)
                {
                    var item = new RemoveAutomationPropertyChangedItem
                    {
                        Property = property,
                        AutoElement = element,
                        EventHandler = eventHandler,
                        WinElement = windowElement
                    };

                    RemoveAutomationPropertyChanged(item);
                    //windowElement.RemoveAutomationPropertyChangedEventHandler(element, property, eventHandler);
                }
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This is the thread which processes requests to add/remove
        /// handlers.  It looks at a queue and processes requests that
        /// are enqueued.
        /// </summary>
        public void HandlerThread()
        {
            while (!_done)
            {
                var obj = _queue.Dequeue();

                if (obj is AddEventHandlerItem)
                {
                    var item = (AddEventHandlerItem)obj;
                    item.WinElement.AddAutomationEventHandler(item.AutoElement, item.AutoEvent, item.EventHandler);
                }
                else if (obj is AddAutomationPropertyChangedItem)
                {
                    var item = (AddAutomationPropertyChangedItem)obj;
                    item.WinElement.AddAutomationPropertyChangedEventHandler(item.AutoElement, item.Property, item.EventHandler);
                }
                else if (obj is RemoveEventHandlerItem)
                {
                    var item = (RemoveEventHandlerItem)obj;
                    item.WinElement.RemoveAutomationEventHandler(item.AutoElement, item.AutoEvent);
                }
                else if (obj is RemoveAutomationPropertyChangedItem)
                {
                    var item = (RemoveAutomationPropertyChangedItem)obj;
                    item.WinElement.RemoveAutomationPropertyChangedEventHandler(item.AutoElement, item.Property, item.EventHandler);
                }
                else if (obj is RemoveAllEventsItem)
                {
                    var item = (RemoveAllEventsItem)obj;
                    item.WinElement.RemoveAllEvents();
                }
                else if (obj is DoneEventHandlerItem)
                {
                    Log.Debug("Received command to quit thread");
                    _done = true;
                }
            }
            Log.Debug("******* EXITING HANDLER THREAD");
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            return true;
        }

        /// <summary>
        /// Creates the thread on which requests will be processed
        /// </summary>
        public void Start()
        {
            _thread = new Thread(HandlerThread);
            _thread.SetApartmentState(ApartmentState.MTA);
            _thread.IsBackground = true;
            _thread.Start();
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    _done = true;

                    var item = new DoneEventHandlerItem();

                    _queue.Enqueue(item);

                    Log.Debug("Aborting thread...");
                    _thread.Abort();
                    Log.Debug("Returned from abort");
                    // Wait until oThread finishes. Join also has overloads
                    // that take a millisecond interval or a TimeSpan object.

                    Log.Debug("Calling Join");
                    _thread.Join();
                    Log.Debug("REturned from join");

                    // dispose all managed resources.
                    Automation.RemoveAllEventHandlers();
                }

                // Release unmanaged resources.
            }
            _disposed = true;
        }

        /// <summary>
        /// Enqueues a request for the item for asynchronous handling
        /// </summary>
        /// <param name="item">item to add</param>
        private static void AddAutomationEvent(AddEventHandlerItem item)
        {
            _instance._queue.Enqueue(item);
        }

        /// <summary>
        /// Enqueues the item for asynchronous handling
        /// </summary>
        /// <param name="item">item to add</param>
        private static void AddAutomationPropertyChanged(AddAutomationPropertyChangedItem item)
        {
            _instance._queue.Enqueue(item);
        }

        /// <summary>
        /// Event handler for from closing.  Remove all its
        /// handlers
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private static void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sender is Form)
            {
                removeAllAutomationEventHandlers((Form)sender);
            }
        }

        /// <summary>
        /// Visibility of the form changed. Remove handlers
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private static void form_VisibleChanged(object sender, EventArgs e)
        {
            if (sender is Form)
            {
                removeAllAutomationEventHandlers((Form)sender);
            }
        }

        /// <summary>
        /// Removes all automation handlers for the specified form
        /// </summary>
        /// <param name="form">the form object</param>
        private static void removeAllAutomationEventHandlers(Form form)
        {
            form.VisibleChanged -= form_VisibleChanged;
            form.FormClosing -= form_FormClosing;

            RemoveAllAutomationEventHandlers(form.Handle);
        }

        /// <summary>
        /// Enqueues the item for asynchronous handling
        /// </summary>
        /// <param name="item">item to add</param>
        private static void RemoveAllEvents(RemoveAllEventsItem item)
        {
            _instance._queue.Enqueue(item);
        }

        /// <summary>
        /// Enqueues a request for the item for asynchronous handling
        /// </summary>
        /// <param name="item">item to add</param>
        private static void RemoveAutomationEvent(RemoveEventHandlerItem item)
        {
            _instance._queue.Enqueue(item);
        }

        /// <summary>
        /// Enqueues the item for asynchronous handling
        /// </summary>
        /// <param name="item">item to add</param>
        private static void RemoveAutomationPropertyChanged(RemoveAutomationPropertyChangedItem item)
        {
            _instance._queue.Enqueue(item);
        }

        /// <summary>
        /// Event handler for window closed.  Remove all
        /// event handlers
        /// </summary>
        /// <param name="hwnd">handle to the window that was closed</param>
        private static void windowElement_EvtOnWindowClosed(IntPtr hwnd)
        {
            Log.Debug(hwnd.ToString());
            RemoveAllAutomationEventHandlers(hwnd);
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class AddAutomationPropertyChangedItem
        {
            public AutomationElement AutoElement;
            public AutomationPropertyChangedEventHandler EventHandler;
            public AutomationProperty Property;
            public WindowElement WinElement;
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class AddEventHandlerItem
        {
            public AutomationElement AutoElement;
            public AutomationEvent AutoEvent;
            public AutomationEventHandler EventHandler;
            public WindowElement WinElement;
        }

        /// <summary>
        /// Blocking queue on which work items can be enqueued.
        /// Dequeue is blocking until something arrives on the queue
        /// </summary>
        /// <typeparam name="T">data type to enqueue</typeparam>
        private class BlockingQueue<T> : IEnumerable<T>
        {
            /// <summary>
            /// The queue to hold the items
            /// </summary>
            private readonly Queue<T> _queue = new Queue<T>();

            /// <summary>
            /// How many itmes are in the queue?
            /// </summary>
            private int _count;

            /// <summary>
            /// Removes next item. If queue is empty,
            /// blocks
            /// </summary>
            /// <returns>next item</returns>
            public T Dequeue()
            {
                lock (_queue)
                {
                    while (_count <= 0) Monitor.Wait(_queue);
                    _count--;
                    return _queue.Dequeue();
                }
            }

            /// <summary>
            /// Enqueues the item and pulses to indicate
            /// there is something there
            /// </summary>
            /// <param name="data">item to enqueue</param>
            public void Enqueue(T data)
            {
                if (data == null) throw new ArgumentNullException("data");
                lock (_queue)
                {
                    _queue.Enqueue(data);
                    _count++;
                    Monitor.Pulse(_queue);
                }
            }

            /// <summary>
            /// Returns enumerator for the queue
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<T>)this).GetEnumerator();
            }

            /// <summary>
            /// Returns enumerator to peek into the queue
            /// </summary>
            /// <returns>enumerator</returns>
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                while (true) yield return Dequeue();
            }
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class DoneEventHandlerItem
        {
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class RemoveAllEventsItem
        {
            public WindowElement WinElement;
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class RemoveAutomationPropertyChangedItem
        {
            public AutomationElement AutoElement;
            public AutomationPropertyChangedEventHandler EventHandler;
            public AutomationProperty Property;
            public WindowElement WinElement;
        }

        /// <summary>
        /// Work item that enqueued for asynchornous processing
        /// </summary>
        private class RemoveEventHandlerItem
        {
            public AutomationElement AutoElement;
            public AutomationEvent AutoEvent;
            public WindowElement WinElement;
        }

        /// <summary>
        /// Keeps track of all the automation handlers for a window
        /// </summary>
        private class WindowElement
        {
            /// <summary>
            /// The elements belonging to this window for which handlers
            /// have been added
            /// </summary>
            private readonly Hashtable _controlElements;

            /// <summary>
            /// The window handle
            /// </summary>
            private readonly IntPtr _hwnd;

            /// <summary>
            /// Initializes an instance fo the class
            /// </summary>
            /// <param name="handle">window handle</param>
            public WindowElement(IntPtr handle)
            {
                _hwnd = handle;

                _controlElements = new Hashtable();
                var windowElement = AutomationElement.FromHandle(_hwnd);
                try
                {
                    Automation.AddAutomationEventHandler(WindowPattern.WindowClosedEvent,
                                                        windowElement,
                                                        TreeScope.Element,
                                                        onWindowClose);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }

            /// <summary>
            /// Delegate for event raised when the window is closed
            /// </summary>
            /// <param name="hwnd">window handle</param>
            public delegate void OnWindowClosed(IntPtr hwnd);

            /// <summary>
            /// Raised when window is closed
            /// </summary>
            public event OnWindowClosed EvtOnWindowClosed;

            /// <summary>
            /// Adds the specified event and event handler for the element
            /// </summary>
            /// <param name="element">automation element</param>
            /// <param name="autoEvent">the event to add</param>
            /// <param name="eventHandler">the event handler</param>
            public void AddAutomationEventHandler(AutomationElement element, AutomationEvent autoEvent, AutomationEventHandler eventHandler)
            {
                Log.Debug();
                try
                {
                    var events = (Hashtable)_controlElements[element];
                    if (events == null)
                    {
                        Log.Debug("events Arraylist is null.  Creating one...");
                        _controlElements.Add(element, new Hashtable());
                        events = (Hashtable)_controlElements[element];
                    }

                    if (!events.Contains(autoEvent))
                    {
                        Log.Debug("Events array does not contain.  Adding automation event " +
                                            autoEvent.ProgrammaticName +
                                            ".  AutomationID: " + (element.Current.AutomationId ?? "none"));

                        Automation.AddAutomationEventHandler(autoEvent, element, TreeScope.Element, eventHandler);
                        Log.Debug("Returned from AddAutomationEventHandler");
                        events.Add(autoEvent, eventHandler);
                        Log.Debug("Done adding");
                    }
                    else
                    {
                        Log.Debug("Event already registered.  Will not be readded" +
                                        autoEvent.ProgrammaticName +
                                        ". AutomationID: " + (element.Current.AutomationId ?? "none"));
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("exception occured!  e=" + e.ToString());
                }
            }

            /// <summary>
            /// Adds a property changed event handler
            /// </summary>
            /// <param name="element">th element for which to add the event handler</param>
            /// <param name="property">the property to track changes for</param>
            /// <param name="eventHandler">the event handler</param>
            public void AddAutomationPropertyChangedEventHandler(AutomationElement element,
                                                                AutomationProperty property,
                                                                AutomationPropertyChangedEventHandler eventHandler)
            {
                Log.Debug();

                try
                {
                    var events = (Hashtable)_controlElements[element]; ;
                    if (events == null)
                    {
                        _controlElements.Add(element, new Hashtable());
                        events = (Hashtable)_controlElements[element];
                    }

                    if (!events.Contains(property))
                    {
                        Automation.AddAutomationPropertyChangedEventHandler(element, TreeScope.Element, onPropertyChanged, property);

                        Log.Debug("Adding property changed event " + property.ProgrammaticName +
                                        ".  AutomationID: " + (element.Current.AutomationId ?? "none"));

                        var eventHandlerList = new List<AutomationPropertyChangedEventHandler>();
                        eventHandlerList.Add(eventHandler);
                        events.Add(property, eventHandlerList);
                    }
                    else
                    {
                        var eventHandlerList = (List<AutomationPropertyChangedEventHandler>)events[property];
                        if (!eventHandlerList.Contains(eventHandler))
                        {
                            Log.Debug("Registering event.  " + property.ProgrammaticName +
                                        ". AutomationID: " + (element.Current.AutomationId ?? "none"));

                            eventHandlerList.Add(eventHandler);
                        }
                        else
                        {
                            Log.Debug("Property change already registered.  " +
                                        property.ProgrammaticName + ". AutomationID: " +
                                        (element.Current.AutomationId ?? "none"));
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            /// <summary>
            /// Removes all automation events associated with the window
            /// </summary>
            public void RemoveAllEvents()
            {
                Log.Debug();

                try
                {
                    Log.Debug("ControlElements count: " + _controlElements.Count);
                    foreach (AutomationElement element in _controlElements.Keys)
                    {
                        var events = (Hashtable)_controlElements[element];
                        Log.Debug("events count: " + events.Count);
                        foreach (var key in events.Keys)
                        {
                            if (key is AutomationEvent)
                            {
                                var autoEvent = key as AutomationEvent;
                                var eventHandler = (AutomationEventHandler)events[autoEvent];
                                try
                                {
                                    Log.Debug("Removing automation event " + autoEvent.ProgrammaticName);
                                    Automation.RemoveAutomationEventHandler(autoEvent, element, eventHandler);
                                    Log.Debug("Done removing automation event");
                                }
                                catch { }
                            }
                            else if (key is AutomationProperty)
                            {
                                var autoProperty = key as AutomationProperty;
                                var eventHandler = (AutomationPropertyChangedEventHandler)events[autoProperty];
                                try
                                {
                                    Log.Debug("Removing automation property " + autoProperty.ProgrammaticName);
                                    Automation.RemoveAutomationPropertyChangedEventHandler(element, eventHandler);
                                    Log.Debug("Done removing automation property");
                                }
                                catch { }
                            }
                        }
                        events.Clear();
                    }
                    _controlElements.Clear();
                }
                catch (Exception e)
                {
                    Log.Debug("exception occured!  e=" + e.ToString());
                }
            }

            /// <summary>
            /// Removes the specified event for the control element
            /// </summary>
            /// <param name="element">the automation element</param>
            /// <param name="autoEvent">the event</param>
            public void RemoveAutomationEventHandler(AutomationElement element, AutomationEvent autoEvent)
            {
                Log.Debug();
                try
                {
                    var events = (Hashtable)_controlElements[element];
                    if (events != null && events.Contains(autoEvent))
                    {
                        Log.Debug("Removing automation event " + autoEvent.ProgrammaticName);
                        var eventHandler = (AutomationEventHandler)events[autoEvent];
                        Automation.RemoveAutomationEventHandler(autoEvent, element, eventHandler);
                        Log.Debug("RemoveAutomationEventHandler succeeded!");
                        events.Remove(autoEvent);
                    }
                    else
                    {
                        Log.Debug("Event already registered.  Will not be readded" +
                                        autoEvent.ProgrammaticName + ". AutomationID: " +
                                        (element.Current.AutomationId ?? "none"));
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            /// <summary>
            /// Removes the property changed event handler for the specified element
            /// </summary>
            /// <param name="element">the element</param>
            /// <param name="property">property to remove</param>
            /// <param name="eventHandler">the event handler for the property</param>
            public void RemoveAutomationPropertyChangedEventHandler(AutomationElement element,
                                                                    AutomationProperty property,
                                                                    AutomationPropertyChangedEventHandler eventHandler)
            {
                Log.Debug();

                try
                {
                    Log.Debug("Removing propertychanged event for automation property " + property.ProgrammaticName);
                    var events = (Hashtable)_controlElements[element];
                    if (events != null && events.Contains(property))
                    {
                        var eventHandlerList = (List<AutomationPropertyChangedEventHandler>)events[property];
                        if (eventHandlerList.Contains(eventHandler))
                        {
                            eventHandlerList.Remove(eventHandler);
                            Log.Debug("RemoveAutomationPropertyChangedEventHandler succeeded!");

                            if (eventHandlerList.Count == 0)
                            {
                                Log.Debug("Event handler list is empty. No more subscribers. Removing event");
                                Automation.RemoveAutomationPropertyChangedEventHandler(element, onPropertyChanged);
                                events.Remove(property);
                            }
                        }
                        else
                        {
                            Log.Debug("Could not remove event.  Did not find event handler in the eventhandlers list");
                        }
                    }
                    else
                    {
                        Log.Debug("Could not remove event.  Did not find property in the events list");
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("exception occured!  e=" + e.ToString());
                }
            }

            /// <summary>
            /// The callback function invoked whenever a property changes.
            /// Notifies all the event subscribers that the property changed
            /// </summary>
            /// <param name="sender">the element that triggered the event</param>
            /// <param name="e">event args</param>
            private void onPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
            {
                Log.Debug(e.Property.ProgrammaticName);

                var element = sender as AutomationElement;
                Hashtable events = (Hashtable)_controlElements[element];

                if (events != null && events.Contains(e.Property))
                {
                    var eventHandlerList = (List<AutomationPropertyChangedEventHandler>)events[e.Property];
                    Log.Debug("eventHandlerList.Count = " + eventHandlerList.Count);
                    foreach (var p in eventHandlerList)
                    {
                        Log.Debug("Calling property changed for " + e.Property.ProgrammaticName);
                        p(sender, e);
                    }
                }
            }

            /// <summary>
            /// Window was closed.  Raise an event to indicate this
            /// </summary>
            /// <param name="sender">event sender</param>
            /// <param name="e">event ards</param>
            private void onWindowClose(object sender, AutomationEventArgs e)
            {
                if (EvtOnWindowClosed != null)
                {
                    Log.Debug("Triggering event closed");
                    EvtOnWindowClosed(_hwnd);
                }
            }
        }
    }
}