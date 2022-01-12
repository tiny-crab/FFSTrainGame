using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MouseAndKeyboard : MonoBehaviour {

    private Datastore _datastore;
    
    public List<KeyCode> pressedKeys = new List<KeyCode>();
    public List<KeyCode> heldKeys = new List<KeyCode>();
    List<KeyCode> watchedKeys = new List<KeyCode> {
        KeyCode.R, KeyCode.E, KeyCode.C, KeyCode.T, KeyCode.B,
    };
    List<KeyCode> watchedHeldKeys = new List<KeyCode> {
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftCommand,
        KeyCode.RightCommand,
        KeyCode.LeftAlt,
        KeyCode.RightAlt,
        KeyCode.Space,
    };
    Dictionary<KeyCode, KeyCode> condensedHeldKeys = new Dictionary<KeyCode, KeyCode> {
        {KeyCode.LeftControl, KeyCode.LeftControl},
        {KeyCode.RightControl, KeyCode.LeftControl},
        {KeyCode.LeftCommand, KeyCode.LeftControl},
        {KeyCode.RightCommand, KeyCode.LeftControl},
        {KeyCode.LeftAlt, KeyCode.LeftAlt},
        {KeyCode.RightAlt, KeyCode.LeftAlt},
        {KeyCode.Space, KeyCode.Space},
    };

    public void Awake() {
        _datastore = GetComponent<Datastore>();
    }

    public void Start() {
        Observable.EveryUpdate()
            .Where(_ => {
                var checkedKeys = watchedKeys.Where(keyCode => Input.GetKeyDown(keyCode)).ToList();
                if (checkedKeys.Count > 0) {
                    pressedKeys = checkedKeys;
                    return true;
                }
                return false;
            })
            .Subscribe(_ => {
                pressedKeys.ForEach(keyCode => {
                    _datastore.inputEvents.Publish(
                        new KeyEvent {
                            keyCode = keyCode,
                            heldKeys = heldKeys,
                        });
                });
            });

        Observable.EveryUpdate()
            .Where(_ => {
                var pressedDownKeys = watchedHeldKeys
                    .Where(keyCode => Input.GetKeyDown(keyCode))
                    .Select(key => condensedHeldKeys[key])
                    .Distinct().ToList();
                var liftedUpKeys = watchedHeldKeys
                    .Where(keyCode => Input.GetKeyUp(keyCode))
                    .Select(key => condensedHeldKeys[key])
                    .Distinct().ToList();
                if (pressedDownKeys.Count > 0 || liftedUpKeys.Count > 0) {
                    var condensedKeys = heldKeys.Except(liftedUpKeys).Concat(pressedDownKeys).ToList();
                    if (heldKeys != condensedKeys) {
                        heldKeys = condensedKeys;
                        return true;
                    }
                }
                return false;
            })
            .Subscribe(_ => {
                _datastore.inputEvents.Publish(
                    new KeyEvent {
                        heldKeys = heldKeys,
                    }
                );
                _datastore.brake.Value = heldKeys.Contains(KeyCode.Space);
            });
    }
}