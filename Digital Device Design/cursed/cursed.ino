#include "InputDebounce.h"

#define BUTTON_DEBOUNCE_DELAY 20    // [ms]

const int out_pin = 5;
const int pot1 = A1;
const int pot2 = A0;
const int pot3 = A2;
const int pin_on_off = 2;
const int pin_plus_octave = 15;
const int pin_minus_octave = 14;
const int pin_change_scale = 3;
const int led_pins[] = { 4, 6, 7, 8 };

InputDebounce button_on_off;
InputDebounce button_plus_octave;
InputDebounce button_minus_octave;
InputDebounce button_change_scale;

const int MIN_FREQUENCY = 130;
const int MAX_FREQUENCY = 4187;
const int A_NOTE = 440;
int frequency;

const int MIN_OCTAVE = -2;
const int MAX_OCTAVE = 2;
int octave = -1;

enum Scale { None, Chromatic, Major, Minor };
Scale scale;
int major_note_numbers[] = { 0, 2, 4, 5, 7, 9, 11, 12 };
int minor_note_numbers[] = { 0, 2, 3, 5, 7, 8, 10, 12 };

float vibrato_amplitude = 0.01;
float vibrato_frequency = 2;
float vibrato_x = 0;
int vibrato_time = 0;


float vibrato_wave()
{
    int new_vibrato_time = millis();
    vibrato_x += (new_vibrato_time - vibrato_time) * 6.28 * vibrato_frequency / 1000.0;
    vibrato_time = new_vibrato_time;

    return random(700, 1300) / 1000.0 * vibrato_amplitude * sin(vibrato_x);

}

void butt_change_scale_callback(uint8_t pinIn)
{
    digitalWrite(led_pins[scale], LOW);
    scale = (Scale)((scale + 1) % 4);
    digitalWrite(led_pins[scale], HIGH);
}

void butt_minus_octave_pressed_callback(uint8_t pinIn)
{
    if (octave > MIN_OCTAVE) octave--;
}

void butt_plus_octave_pressed_callback(uint8_t pinIn)
{
    if (octave < MAX_OCTAVE) octave++;
}

void butt_on_off_pressed_callback(uint8_t pinIn)
{
    tone(out_pin, frequency);
}

void butt_on_off_released_callback(uint8_t pinIn)
{
    noTone(out_pin);
}

int get_note_pitch(int pot_value)
{
    float h = 12 * octave + 3.0;  // C note of selected octave
    switch (scale)
    {
        case None:
            h += pot_value / 1024.0 * 12.0;
            break;
        case Chromatic:
            h += constrain(map(pot_value, 0, 1023, 0, 13), 0, 12);
            break;
        case Major:
            h += major_note_numbers[constrain(map(pot_value, 0, 1023, 0, 8), 0, 7)];
            break;
        case Minor:
            h += minor_note_numbers[constrain(map(pot_value, 0, 1023, 0, 8), 0, 7)];
            break;
        default:
            break;
    }

    float pitch = pow(2, h / 12.0) * A_NOTE;
    pitch += vibrato_wave() * (float)pitch;
    return pitch;
}

float get_vibrato_frequency(int pot_value)
{
    return map(pot_value, 0, 1023, 10, 100) / 10.0;
}

float get_vibrato_amplitude(int pot_value)
{
    return pot_value / 1023.0 * 0.02;
}

void setup() {
    // put your setup code here, to run once:
    pinMode(out_pin, OUTPUT);
    for (int i = 0; i < 4; i++)
    {
        pinMode(led_pins[i], OUTPUT);
    }
    scale = None;
    digitalWrite(led_pins[scale], HIGH);

    button_on_off.registerCallbacks(butt_on_off_pressed_callback, butt_on_off_released_callback, butt_on_off_pressed_callback, NULL);
    button_plus_octave.registerCallbacks(butt_plus_octave_pressed_callback, NULL, NULL, NULL);
    button_minus_octave.registerCallbacks(butt_minus_octave_pressed_callback, NULL, NULL, NULL);
    button_change_scale.registerCallbacks(butt_change_scale_callback, NULL, NULL, NULL);

    button_on_off.setup(pin_on_off, BUTTON_DEBOUNCE_DELAY, InputDebounce::PIM_INT_PULL_UP_RES);
    button_plus_octave.setup(pin_plus_octave, BUTTON_DEBOUNCE_DELAY, InputDebounce::PIM_INT_PULL_UP_RES);
    button_minus_octave.setup(pin_minus_octave, BUTTON_DEBOUNCE_DELAY, InputDebounce::PIM_INT_PULL_UP_RES);
    button_change_scale.setup(pin_change_scale, BUTTON_DEBOUNCE_DELAY, InputDebounce::PIM_INT_PULL_UP_RES);

    pinMode(pot1, INPUT);
    pinMode(pot2, INPUT);
    pinMode(pot3, INPUT);

    Serial.begin(200000);
}

void loop() {
    // put your main code here, to run repeatedly:
    unsigned long now = millis();

    button_on_off.process(now);
    button_plus_octave.process(now);
    button_minus_octave.process(now);
    button_change_scale.process(now);

    frequency = get_note_pitch(analogRead(pot1));
    vibrato_frequency = get_vibrato_frequency(analogRead(pot2));
    vibrato_amplitude = get_vibrato_amplitude(analogRead(pot3));

    Serial.print(octave);
    Serial.print(" ");
    Serial.print(scale);
    Serial.print(" ");
    Serial.print(vibrato_frequency);
    Serial.print(" ");
    Serial.println(frequency);
}
