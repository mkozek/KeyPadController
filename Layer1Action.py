import usb_hid
from adafruit_hid.keyboard import Keyboard
from adafruit_hid.keyboard_layout_us import KeyboardLayoutUS
from adafruit_hid.keycode import Keycode
from adafruit_hid.consumer_control import ConsumerControl
from adafruit_hid.consumer_control_code import ConsumerControlCode
keyboard = Keyboard(usb_hid.devices)
layout = KeyboardLayoutUS(keyboard)
consumer_control = ConsumerControl(usb_hid.devices)


def Layer1(k):
    if k == 4:
        keyboard.send(keycode.ALT, keycode.F)
    elif k == 5:
        consumer_control.send(ConsumerControlCode.MUTE)
    elif k == 6:
        consumer_control.send(ConsumerControlCode.MUTE)
    elif k == 7:
        consumer_control.send(ConsumerControlCode.MUTE)



