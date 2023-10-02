# SoloLeveling2DGame
A 2d game I'm making for fun in Unity

## Controls

### Gameplay

<kbd>&#8592;</kbd>, <kbd>&#8594;</kbd> to move

<kbd>A</kbd> = jump (causes shockwave, and an earth prism will rise from the ground); release early to produce aerodynamic heat, stopping your ascent

<kbd>S</kbd> = attack

<kbd>D</kbd> = dodge roll (costs 20 mana, causes a lightning attack)

<kbd>Q</kbd> = toggle lightning fist on or off (consumes 1.5 mana per second)

<kbd>R</kbd> = add a stack of berserker ferocity (costs 40 mana, lasts 20 seconds)

<kbd>esc</kbd> = open pause menu (must use mouse navigation while open)

<kbd>tab</kbd> = open/close inventory (currently not functional)

<kbd>T</kbd> = open/close status menu (very important)

### Menu navigation

<kbd>A</kbd> = select

<kbd>Q</kbd> = back

<kbd>esc</kbd> = exit

<kbd>&#8592;</kbd>, <kbd>&#8593;</kbd>, <kbd>&#8594;</kbd>, <kbd>&#8595;</kbd> to highlight different buttons in a particular menu or sub-menu

Note: pause menu and main menu still use mouse navigation, unlike the inventory and status menu

## Status Menu

This menu is currently mostly placeholder text, but the Attribute Sub-menu is functional.

### Attribute Sub-menu

You can allocate attribute points in the Attribute Sub-menu, and set stat caps to determine how those points are spent.

Your health pool is increased by allocating points into Strength, and mana pool is increased by allocating points into Stamina.

## Damage Calculation

### Base Damage

Physical and geomancy attacks use physical damage as their base, while fire and lightning attacks use magical damage as their base.

Physical damage is increased by allocating points into Strength, while magical damage is increased by allocating points into Intelligence.

### Damage Scaling

Each class' skill damage scales off a particular stat.

| Attribute    | Classes That Scale With It |
|--------------|----------------------------|
| Strength     | Geomancer, Bloodmage       |
| Stamina      | Berserker                  |
| Agility      | Electromancer              |
| Intelligence | Pyromancer                 |
| Perception   |                            |

## Known Issues

- Melee attacks can only be initiated while touching the ground
- Running out of mana does not deactivate abilities that consume it over time
- Ferocity recovery animation acts weirdly if you do a fraction of a jump at any point in attack animation

## Recommended Settings (for min-maxing benefits of point allocation)

In the attribute menu, I recommend setting Crit Rate Cap at 50% of points, and Movement Speed Cap at 300% constant value. For attack rate, combos can be difficult above 1000%. I encourage you to play around with these values and find something that works for your playstyle.
