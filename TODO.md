# TODO-List

## Urgent Tasks

~~Add a `OnExternalGestureCompleted` event to `IGestures` with as parameter an GestureCompletedEventArgs that contains the gesture type and the gesture data.~~

## Medium Priority Tasks

### Bindings Rework

- [ ] Wrap different versions of Binding under a single generic interface, with a `Press()` and `Release()` method.

### Debugger

- [ ] Implement an In-app gesture debugger, draw the inputs as well as state changes when they happen. (Simillar to osu!lazer's replay analyzer or Rewind)

## Documentation

### Dev Documentation (Github Wiki)

For each of these topics, indicates in which way such systems could be improved, and what the current limitations are.

- [ ] Architecture Overview
    - [ ] Gesture Handler
    - [ ] Gesture Daemon
    - [ ] Gesture Tiles
    - [ ] Gesture Setup
    - [ ] Gesture Unit Tests

- [ ] Intended behaviors
    - [ ] UX
        - [ ] Connection Screen
        - [ ] Binding Overview (Main View)
        - [ ] Gesture Setup
            - [ ] Gesture Selection Screen
            - [ ] Gesture Setup Screen
                - [ ] Options Selection Screen
                - [ ] Binding Selection Screen
                - [ ] Tweaks Screen (Area & Gesture specific tweaks)
        - [ ] Gesture Editing
        - [ ] Gesture Removal

    - [ ] Gestures
        - [ ] Tap
        - [ ] Hold
        - [ ] Swipe
        - [ ] Pan
        - [ ] Pinch
        - [ ] Rotate

- [ ] UX Testing Procedures
- [ ] Gesture Testing Procedures
    - [ ] Unit Tests
    - [ ] Manual Testing

### User Documentation (Github Pages)

- [ ] A download link for the current platform & another to the github releases page

- [x] Installation Guide
    - [x] Dependencies
    - [x] Installation

- [x] Supported Gestures

- [x] Getting Started
    - [x] Binding Overview Screen
    - [x] Adding Gestures
    - [x] Editing Gestures
    - [x] Deleting Gestures

- [ ] Advanced
    - [ ] Node-Based Gestures
    - [ ] Gesture Recording
    - [ ] Gesture Recognition

- [-] FAQ
    - [-] Basic
    - [-] Troubleshooting
