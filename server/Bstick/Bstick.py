from blinkstick import blinkstick
import random
import time


class Stick(blinkstick.BlinkStickPro):

    def change_led_color(self, colors):
        self.clear()
        print ('Changing colors to: ', colors[0:3])
        for x in range(0, self.r_led_count):
            self.set_color_or(x, int(colors[0]), int(colors[1]), int(colors[2]))
        self.send_data_all()
        time.sleep(0.020)
        return 'ok'

    def change_led_random(self):
        self.clear()
        print ('Setting a random color to all leds')
        for x in range(0, self.r_led_count):
            rr = random.randint(0, 255)
            rg = random.randint(0, 255)
            rb = random.randint(0, 255)
            self.set_color_or(x, rr, rb, rg)

        self.send_data_all()
        time.sleep(0.020)
        return 'ok'

    def spaz_out(self):
        random_led = random.randint(0, self.r_led_count-1)

        rr = random.randint(0, 255)
        rg = random.randint(0, 255)
        rb = random.randint(0, 255)

        self.set_color_or(random_led, rr, rb, rg)
        self.send_data_all()
        time.sleep(0.003)

    def set_color_or(self, x, r, g, b):
        #cr, cg, cb = self.get_color(0, x)
        #self.set_color(0, x, int(r) | int(cr), int(g) | int(cg), int(b) | int(cb))
        self.set_color(0, x, int(r), int(g), int(b))
