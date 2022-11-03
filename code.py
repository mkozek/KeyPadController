# This code is based on official pimoroni sample for pico RGB keyboard

# You'll need to connect Keybow 2040 to a computer, as you would with a regular
# USB keyboard.

# Drop the `pmk` folder
# into your `lib` folder on your `CIRCUITPY` drive.

# NOTE! Requires the adafruit_hid CircuitPython library also!

import time
from pmk import PMK
from pmk.platform.rgbkeypadbase import RGBKeypadBase as Hardware  # for Pico RGB Keypad Base

import usb_hid
from adafruit_hid.keyboard import Keyboard
from adafruit_hid.keyboard_layout_us import KeyboardLayoutUS
from adafruit_hid.keycode import Keycode

from adafruit_hid.consumer_control import ConsumerControl
from adafruit_hid.consumer_control_code import ConsumerControlCode
import Layer1Action
from Layer1Color import Color1

# Set up Keybow
keybow = PMK(Hardware())
keys = keybow.keys

# Set up the keyboard and layout
keyboard = Keyboard(usb_hid.devices)
layout = KeyboardLayoutUS(keyboard)

# Set up consumer control (used to send media key presses)
consumer_control = ConsumerControl(usb_hid.devices)

# Our layers. The key of item in the layer dictionary is the key number on
# Keybow to map to, and the value is the key press to send.

# Note that keys 0-3 are reserved as the modifier and layer selector keys
# respectively.

layer_1 = {4: Keycode.ZERO,
           5: Keycode.ONE,
           6: Keycode.FOUR,
           7: Keycode.SEVEN,
           8: Keycode.DELETE,
           9: Keycode.TWO,
           10: Keycode.FIVE,
           11: Keycode.EIGHT,
           12: Keycode.ENTER,
           13: Keycode.THREE,
           14: Keycode.SIX,
           15: Keycode.NINE}

layer_2 = {8: "win+r",
           12: "wt",
           5: "TIANumericMod",
           6: "TIABoolTrue",
           7: "TIABoolFalse"}

layer_3 = {14: ConsumerControlCode.VOLUME_DECREMENT,
           7: ConsumerControlCode.SCAN_PREVIOUS_TRACK,
           10: ConsumerControlCode.MUTE,
           11: ConsumerControlCode.PLAY_PAUSE,
           6: ConsumerControlCode.VOLUME_INCREMENT,
           15: ConsumerControlCode.SCAN_NEXT_TRACK,
           5: ConsumerControlCode.BRIGHTNESS_INCREMENT,
           13: ConsumerControlCode.BRIGHTNESS_DECREMENT}

layers = {1: layer_1,
          2: layer_2,
          3: layer_3}

# Define the modifier key and layer selector keys
modifier = keys[0]

selectors = {1: keys[1],
             2: keys[2],
             3: keys[3]}

# Start on layer 1
current_layer = 1

# The colours for each layer
colours = {1: (75, 0, 75),
           2: (0, 50, 75),
           3: (100, 50, 0)}

layer_keys = range(4, 16)

# Set the LEDs for each key in the current layer
for k in layers[current_layer].keys():
    keys[k].set_led(*colours[current_layer])

# To prevent the strings (as opposed to single key presses) that are sent from
# refiring on a single key press, the debounce time for the strings has to be
# longer.
short_debounce = 0.03
long_debounce = 0.15
debounce = 0.03
fired = False

while True:
    # Always remember to call keybow.update()!
    keybow.update()

    # This handles the modifier and layer selector behaviour
    if modifier.held:
        # Give some visual feedback for the modifier key
        keys[0].led_off()

        # If the modifier key is held, light up the layer selector keys
        for layer in layers.keys():
            keys[layer].set_led(*colours[layer])

            # Change layer if layer key is pressed
            if current_layer != layer:
                if selectors[layer].pressed:
                    current_layer = layer

                    # Set the key LEDs first to off, then to their layer colour
                    for k in layer_keys:
                        keys[k].set_led(0, 0, 0)

                    for k in layers[layer].keys():
                        if current_layer == 1:
                            if k not in Color1:
                                keys[k].set_led(0,0,0)
                            else:
                                keys[k].set_led(*Color1[k])

                        else:
                            keys[k].set_led(*colours[layer])

    # Turn off the layer selector LEDs if the modifier isn't held
    else:
        for layer in layers.keys():
            keys[layer].led_off()

        # Give some visual feedback for the modifier key
        keys[0].set_led(0, 100, 25)

    # Loop through all of the keys in the layer and if they're pressed, get the
    # key code from the layer's key map
    for k in layers[current_layer].keys():
        if keys[k].pressed:
            key_press = layers[current_layer][k]

            # If the key hasn't just fired (prevents refiring)
            if not fired:
                fired = True

                # Send the right sort of key press and set debounce for each
                # layer accordingly (layer 2 needs a long debounce)
                if current_layer == 1:
                    Layer1Action.Layer1(k)
                elif current_layer == 2:
                    debounce = long_debounce
                    if k == 8:
                        keyboard.send(Keycode.WINDOWS, Keycode.R)
                    elif k == 12:
                        keyboard.send(Keycode.CONTROL, Keycode.ALT, Keycode.W)
                    elif k == 5:
                        keyboard.send(Keycode.CONTROL, Keycode.SHIFT, Keycode.TWO)
                    elif k == 6:
                        keyboard.send(Keycode.CONTROL, Keycode.F2)
                    elif k == 7:
                        keyboard.send(Keycode.CONTROL, Keycode.F3)
                    else:
                        layout.write(key_press)

                elif current_layer == 3:
                    debounce = short_debounce
                    consumer_control.send(key_press)

    # If enough time has passed, reset the fired variable
    if fired and time.monotonic() - keybow.time_of_last_press > debounce:
        fired = False
