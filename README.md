# Unity-2.5D-Destruction
Unity 2.5D Destruction is a basic tool for breaking 2D sprites into 2.5D fragments for awesome destruction effects!!!

## Instructions for Basic Use
* Import the Unity 2.5D Destruction package
* drag a sprite into your scene
* Add an Explodable component
* Set your parameters and click Generate Fragments (repeat until you are satisfied with your fragments)
* During gameplay, call explode() on the Explodable component to destroy the original sprite and activate the fragments

## Detailed Explanation of Parameters
**Allow Runtime Fragmentation**: Set this to true to generate your fragments during gameplay instead of in the editor. When you call "explode()" the fragments will be generated and the original destroyed.
The fragmentaiton operation isn't the fastest so I don't really recommend this.

**DestroyAfterHit**: Can set this to automatically destroy the pieces (can be extended for random duration).

**Collider Type**: Can select a type of collider for the parent.

**Parent Collider Width**: Can select a width of the collider on the parent (results may vary, double-check).

**Children Collider Width**: Can select a width of the collider on the children (results may vary, double-check).

**Shatter Type**: Can set this to generate triangular fragments or more "realistic" voronoi fragments.

**Extra Points**: Ordinarilly the fragments are generated using the points of the collider. With this parameter you can add any number of random points inside the bounds of the collider.
Use this to get more random and interesting pieces.

**Subshatter Steps**: For each subshatter step, the fragmentation operation will be run on each generated fragment. For example if it's set to 2 your sprite will be fragmented, then each fragment
will be fragmented, and then those fragments will be fragmented again. I wouldn't set this above 2 and usually 1 is enough.

**Fragment Layer**: The layer you wish the fragments to be set to

**Sorting Layer**: The sorting layer you wish the fragments to be set to

**Order In Layer**: The order in layer you wish the fragments to be set to

Based on: https://github.com/mjholtzem/Unity-2D-Destruction

Credit to mjholtzem for most of the logic!