// GENERATED AUTOMATICALLY FROM 'Assets/_Project/Input/controls_default.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Misadventure.Input
{
    public class @DefaultControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @DefaultControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""controls_default"",
    ""maps"": [
        {
            ""name"": ""map_default"",
            ""id"": ""02932347-4c8f-4982-8d05-72d981bf86ea"",
            ""actions"": [
                {
                    ""name"": ""action_finish"",
                    ""type"": ""Button"",
                    ""id"": ""ca036363-2756-4a03-a536-2cdfecc6bf7a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6a93c6f4-9f01-41a6-83f1-f0b197ab1311"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""action_finish"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // map_default
            m_map_default = asset.FindActionMap("map_default", throwIfNotFound: true);
            m_map_default_action_finish = m_map_default.FindAction("action_finish", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // map_default
        private readonly InputActionMap m_map_default;
        private IMap_defaultActions m_Map_defaultActionsCallbackInterface;
        private readonly InputAction m_map_default_action_finish;
        public struct Map_defaultActions
        {
            private @DefaultControls m_Wrapper;
            public Map_defaultActions(@DefaultControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @action_finish => m_Wrapper.m_map_default_action_finish;
            public InputActionMap Get() { return m_Wrapper.m_map_default; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(Map_defaultActions set) { return set.Get(); }
            public void SetCallbacks(IMap_defaultActions instance)
            {
                if (m_Wrapper.m_Map_defaultActionsCallbackInterface != null)
                {
                    @action_finish.started -= m_Wrapper.m_Map_defaultActionsCallbackInterface.OnAction_finish;
                    @action_finish.performed -= m_Wrapper.m_Map_defaultActionsCallbackInterface.OnAction_finish;
                    @action_finish.canceled -= m_Wrapper.m_Map_defaultActionsCallbackInterface.OnAction_finish;
                }
                m_Wrapper.m_Map_defaultActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @action_finish.started += instance.OnAction_finish;
                    @action_finish.performed += instance.OnAction_finish;
                    @action_finish.canceled += instance.OnAction_finish;
                }
            }
        }
        public Map_defaultActions @map_default => new Map_defaultActions(this);
        public interface IMap_defaultActions
        {
            void OnAction_finish(InputAction.CallbackContext context);
        }
    }
}
