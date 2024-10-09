## BG-KoKa - Hide spectators from cheat applications

# Theory: We should be able to hide the spectating players via not transmitting their `observerPawns` as that way the cheat application can never get the `observerPawns`'s member `m_hObserverTarget`

# Actually: There was one test with a paid cs2 cheat and the person seemed trust worthy as the test was laid in simple terms and without hiding the objective, in cs2 mod communitu; 
# Result: The spectator's(me) didn't show on the tester screen
# Conclusion: most likely the cheat receives wrong/broken entity indexes when dereferencing the `m_hObserverTarget`'s <CBaseEntity> pointer; to be personally tested 