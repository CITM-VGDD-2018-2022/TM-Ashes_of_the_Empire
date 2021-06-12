using System;
using DiamondEngine;
using System.Collections.Generic;

public enum DialogImages
{
    IMG_NONE = -1,
    IMG_MANDO_SERIOUS = 0,
    IMG_MANDO_ASHAMED,
    IMG_MANDO_ANGRY,
    IMG_MANDO_THREATENING,
    IMG_MANDO_JOKING,
    IMG_MANDO_HAPPY,
    IMG_GROGU_SERIOUS,
    IMG_GROGU_LAUGHING,
    IMG_GROGU_CURIOUS,
    IMG_GROGU_SCARED,
    IMG_BOKATAN_SERIOUS,
    IMG_BOKATAN_ANGRY,
    IMG_CARA_SERIOUS,
    IMG_CARA_ANGRY1,
    IMG_CARA_ANGRY2,
    IMG_CARA_ASHAMED,
    IMG_GREEF_SERIOUS,
    IMG_GREEF_LAUGHING,
    IMG_GREEF_THINKER,
    IMG_GREEF_ASHAMED,
    IMG_AHSOKA_SERIOUS,
    IMG_AHSOKA_THINKER,
    IMG_AHSOKA_MELANCHOLIC,
    IMG_MOFF_SERIOUS,
    IMG_MOFF_JESTER,
    IMG_MOFF_ANGRY,
    IMG_MOFF_THREATENING1,
    IMG_MOFF_THREATENING2,
    IMG_MOFF_DEFEATED,
    IMG_MAX
}

public class List_Of_Dialogs : DiamondComponent
{

    private List<string> Final_Cutscene = new List<string>()
    {
        "Look who we got here. I honestly didn't know if you were going to make it.",
        "Wow, thanks for the trust you put in me. Also, pretty bold to say that for the woman that was too scared to  do it herself. Anyways, I did it, so could you please tell me...",
        "Don't you dare say I was scared of a simple beast again! I am a true Mandalorian warrior, you'd know what that means if you were really one of us, and not  one of those fools from your cult. If I asked you to do  it it's just because I wanted to test you.",
        "Test me? Test me for what?",
        "Well, as you already know, I'm trying to retake Mandalore. I just wanted to see if you were a worthy warrior, strong enough to help us bring our home world back to its glory days. And you have proven yourself indeed, so I want you to accompany us in this important mission.",
        "Look, everyone knows Mandalore is cursed, i wouldn't go there if I were you. Besides that I already have a mission, I have to protect this kid. And to do that I need the information you promised me, so if you are  kind enough...",
        "I see... Let's make another deal. I'll give you the information you need if, in exchange, you help me retake Mandalore once you are done with your 'far more important' mission. I think your abilities would    really help us, and it's your duty if you truly are a Mandalorian.",
        "Wait, this doesn't seem fair. We already made a deal, remember?",
        "Come on, was this even an inconvenience to you, killing that Rancor? Because if that's the case maybe I got the wrong person for the job. Maybe you are not atrue Mandalorian warrior after all...",
        "Alright, alright.. I'll help you retake Mandalore, but only after I'm done with my mission. No matter how long it takes.",
        "Ok Mando, as you wish. I'll be waiting for you to finishwhatever it is you're doing, gathering as many warriors I can to help us. Now to the information you  wanted; our fellow friend Ahsoka Tano. Can I ask whyyou want to know where she is?",
        "No, you cannot. I need her help with the kid, that's what you need to know.",
        "Fair enough, I guess it's none of my business. She is at Hoth, last thing I heard she was helping the rebelion with some remnants of the Empire that dig in after they lost their base. Those damn imperials, even now they are still giving trouble to the new Republic.",
        "That's why I want to retake Mandalore, I can't stand here dishonoring the deaths amoung our people. I can vividly remember the day we overthrew that Zabrak. The celebrations we made and how we thought that will be the start of a new era to our people...",
        "*Snaps out* Look I don't want to bore you with old stories. The important thing is that you should be careful there.",
        "It's ok, I'll be fine. Thanks for everything, Bo-Katan.",
        "You're welcome. Can I ask for one last thing before you go?",
        "I hope it's not another one of your deals.",
        "Nothing of that. When you find Ahsoka, could you askher if she'd like to help us with our mission?",
        "No problem. See you soon, Bo-Katan.",
        "May the force be with you, friend.",
    };

    private List<bool> Final_Cutscene_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Final_Cutscene_images = new List<DialogImages>()
    {
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Hoth_Cutscene = new List<string>()
    {
        "Thank you for your help, is nice to see you again mandalorian, and you too little fella. What bussiness brings you to this desolate planet?",
        "You are welcome togruta. My mission is to gather regarding the whereabouts of Moff Gideon, I've been told that you could help me.",
        "Indeed, I'm well informed in imperial matters. But be careful, you are pursuing a dangerous man, one of the few serious threats after the deminishing of the Empire.",
        "I don't care about the damage that he can deal to the galaxy, my only purpose is to end him in order to assure the kid's well being.",
        "Your connection with that kid is strong Mando. I've seen a lot of suffering through my life so I can underestand what is wanting to protect someone",
        "Here, take this tracking device. Gideon is always moving, he hardly leaves his light cruiser, so he won’t get detected. This tracking device points to the ship’s location, that seems to currently be.. somewhere in the Bespin System. ",
        "Thanks, I know that you care about the kid aswell. I assure you I will end this.",
        "I can feel your determination. Good luck Mando",
        "Wait, before you go, Bo-Katan asked me to request your help in her upcoming plan of retaking Mandalore.",
        "She will never change, will she? Always determined to reestablish the glory of her people. You can tell her that I will be there, it will be like old times...",
        "Like old times?",
        "Hadn't she tell you about how we retook Mandalore from a sith lord at the end of the Clone Wars",
        "Wait, you've been to Mandalore?",
        "Several times, my friend, but that's a story for another occasion. You have a mission, remember? Once you are done, I’ll tell you everything I know about Mandalore. But now, you have to go.",
        "You’re right, I should get going. Thank you for everything, my friend.",
        "May the force be with you and our little friend.",
    };

    private List<bool> Hoth_Cutscene_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,

    };

    private List<DialogImages> Hoth_Cutscene_images = new List<DialogImages>()
    {
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_SERIOUS,
    };

    private List<string> Initial_Cutscene_Dialogue = new List<string>()
    {
        "I'm not sure why they want to meet up in that dunghill, it's not even close to Mandalore. I don't know about this, kid, curse the time when I agreed to that plan.",
        "(The kid looks at Mando with his deep dark eyes, you can't be sure if he did underestand something of what he just said).",
    };

    private List<bool> Initial_Cutscene_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
    };

    private List<DialogImages> Initial_Cutscene_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_CURIOUS,
    };

    private List<string> Post_Initial_Cutscene_Dialogue = new List<string>()
    {
        "Mando, my friend! Come, I've grabbed you something to drink! It's been too long!",
        "I've heard some impressive rumours lately about you... Mind to confirm some juicy details?",
    };

    private List<bool> Post_Initial_Cutscene_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        false,
    };

    private List<DialogImages> Post_Initial_Cutscene_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_GREEF_LAUGHING,
    };

    private List<string> Ahsoka_PreBoss_Dialogue = new List<string>()
    {
        "These are the lasts steps to assure the youngling's safety I'll be no jedi anymore but I can sense your victory.",
        "May the force be with you my friend.",
        "Thank you Ahsoka, I won't lose.",
    };

    private List<bool> Ahsoka_PreBoss_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        false,
        true,
    };

    private List<DialogImages> Ahsoka_PreBoss_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Cara_PreBoss_Dialogue = new List<string>()
    {
        "Here we are Djin. show that filthy Imperial what happens when he mess with our little friend.",
        "I will Cara, for the kid and all the suffering he has caused to the people across the galaxy.",
        "Amen.",
    };

    private List<bool> Cara_PreBoss_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false
    };

    private List<DialogImages> Cara_PreBoss_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_CARA_SERIOUS,
    };

    private List<string> Greef_PreBoss_Dialogue = new List<string>()
    {
        "I hope at least you will buy me a drink after this.",
        "Count on it Greef.",
        "Also a new pair of boots would be nice...",
        "Don't push your luck...",
    };

    private List<bool> Greef_PreBoss_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_PreBoss_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
    };

    private List<string> Bo_Katan_PreBoss_Dialogue = new List<string>()
    {
        "Finish this quickly, we have a planet to retake.",
        "I'll take that as your way to encourage me...",
        "Whatever you want, but go now.",
    };

    private List<bool> Bo_Katan_PreBoss_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_PreBoss_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Grogu_PreBoss_Dialogue = new List<string>()
    {
        "We end this here kid, you will be safe afterwards. I promise.",
        "(Far from being afraid the kid looks at Mando with determination).",
    };

    private List<bool> Grogu_PreBoss_Dialogue_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
    };

    private List<DialogImages> Grogu_PreBoss_Dialogue_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SERIOUS,
    };

    private List<string> Initial_Cutscene = new List<string>()
    {
        "Well kid, we should get going if we want to kill that Rancor before nightfall.",
        "(Looks at him, without any light of understandment shining in his eyes)",
        "You don't remember? We agreed with that woman that if we defeated the Rancor that escaped from Jabba's palace she would help us find the Jedi.",
        "(Closes his eyes, as if just hearing that name scared him)",
        "It's ok kid, don't worry, he's not here now, he cannot harm you. But we have to defeat him so he leaves us   alone, you know he won't rest until we stop him.",
        "(Slowly calms and opens his eyes, still scared, and looks at MANDO)",
        "Let's go now, kid. I smell trouble.",
    };

    private List<bool> Initial_Cutscene_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Initial_Cutscene_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
    };


    //////////////////////////////////////////////////////////////////////////////////////// BO KATAN ////////////////////////////////////////////////////////////////////////////////////////
    private List<string> Bo_Katan_0_1 = new List<string>()
    {
        "What a coincidence. Taking a break from your reclaiming of Mandalore?",
        "Don't you even dare to utter that name, bounty hunter.",
        "It is nice to see you too.",
        "Tss...",
        "I wonder why do you care so much for that pile of rubble.",
        "You wouldn't understand, you weren't there... You just wait and see, bounty hunter",
    };

    private List<bool> Bo_Katan_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_0_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_0_2 = new List<string>()
    {
        "Why did you really come here? Was it to convince me to help you in your campaign?",
        "Maybe. You made a promise, remember? I helped you find Moff Gideon.",
        "You pointed a certain direction and called it a day. Is that what you call help?",
        "A deal is a deal. Are you backing out from your word?",
        "No, a deal is a deal. I always honour my word.",
    };

    private List<bool> Bo_Katan_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Bo_Katan_0_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
    };

    private List<string> Bo_Katan_0_3 = new List<string>()
    {
        "Isn't there some other way I could help you?",
        "If you're not going to fight like a Mandalorian, give us your armor. A true warrior will use it in your stead.",
        "You know that's never going to happen.",
        "Then this means you will fight with us, right?",
        "You are ruthless, woman. Do I really have no other option?",
        "If you consider yourself to be a Mandalorian, no you don't.",
    };

    private List<bool> Bo_Katan_0_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_0_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_1_1 = new List<string>()
    {
        "Am I supposed to drink this? It is unapologetically disgusting.",
        "Oh, excuse me your highness. Is this not worthy enough of royal stomach?",
        "Very funny... And yet, not entirely wrong. I am actually of clan Kryze, one of the ruling houses. ",
        "Once upon a time, my people used to call me Mand'alor... the title of a Queen",
        "Once upon a time? What happened?",
        "Well, the Empire happened. And I... We lost everything. For them it was 'The Great Purge'. For us? It was a massacre",
        "I've heard rumors about it. My tribe described it as a genocide...",
        "Yes, it was... It truly was...",

    };

    private List<bool> Bo_Katan_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_1_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_1_2 = new List<string>()
    {
        "Say Bo-Katan, now that the Empire is defeated, why can't you reclaim your title?",
        "I'm afraid it is not that easy. There are some political implications to that, and even worse... the Empire is not truly gone.",
        "No... I guess not. I wouldn't have had half the trouble protecting Grogu if it were.",
        "They are still lurking in the shadows, unnoticed, unseen. I fear for the future to come, bounty hunter",
    };

    private List<bool> Bo_Katan_1_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_1_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_1_3 = new List<string>()
    {
        "Hye, I've been thinking about our last chat. Why did you say that it's not that easy to bring all the mandalorians together?",
        "The title of Mand'alor is not given, it is earned, and in the same way as it is earned, it can be lost.",
        "There's a sacred weapon, the Darksaber: It's owner has the right to rule. Moff Gideon took it from me the day we lost our home.",
        "The Darksaber?",
        "It is a powerful ancient weapon that belongs to the mandalorians. I must get it back, Mando. I must take what's mine",
    };

    private List<bool> Bo_Katan_1_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_1_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_2_1 = new List<string>()
    {
        "Another one? I already told you I cannot drink this, just looking at it is unpleasant.",
        "You are welcome. By the way, what happened the last time you acquired the Darksaber?",
        "I was blinded, young and stupid. That's what happened. ",
        "I accepted the Darksaber from Sabine Wren, a fellow Mandalorian and rightful owner of the blade. ",
        "I see nothing wrong with that. If everyone agreed, you did the right thing.",
        "They surely agreed at that time. However, when the great purge occurred and I lost Mandalore, they changed their mind. ",
        "Given that I had not really earned the weapon, they thought I had been an illegitimate ruler and we had lost because of my weakness.",
        "It was not your fault. The moment the Empire put its gaze on Mandalore, your fate was sealed.",
        "I do not need your pity, bounty hunter... I need your help.",
    };

    private List<bool> Bo_Katan_2_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_2_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_2_2 = new List<string>()
    {
        "How did you gain the people's approval to become their new leader? I'm guessing not thanks to your charming skills...",
        "Like you are one to talk. Stop trying to develop a sense of humour, thanks. That is a long story.",
        "I have time.",
        "*Sigh*, Then, long story short: my sister had been the leader of Mandalore for a long time and after her death, I was next in line.",
        "I had already been appointed regent for a small period of time. I also helped reclaim Mandalore from the hands of a clan that had bowed to the empire and ruled Mandalore under their supervision. And there were other things...",
        "You really didn't make that up right now did you? And here I thought I had lived an exciting life...",
        "I already told you; you know nothing of Mandalore.",
    };

    private List<bool> Bo_Katan_2_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_2_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_2_3 = new List<string>()
    {
        "Hold on a second, to have lived through so much... How old are you?",
        "Well, that is rude. But coming from you, should I be surprised? I assume they do not teach manners in your cult.",
        "*Tsch* The Tribe is not a cult. If not for them I would be dead, you know?! I owe them everything. They are good people... But seriously, when were you born?",
        "You are hopeless. I will not tell you.",
        "Ohh pleaseee.",
        "Stop being annoying.",
        "Ok, I'll guess then. You must be... in your mid-forties... No, early fifties?",
        "You impertinent cheeky...",
        "Aha, so I did get it right. It's incredible what cosmetic surgery can do these days...",
        "If I ever meet you alone, you are dead.",
    };

    private List<bool> Bo_Katan_2_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_2_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_3_1 = new List<string>()
    {
        "Stop giving me this awful beverage. I am starting to think you want to poison me.",
        "Tempting, but poison is not the way of the Mandalore. Besides, Bantha Milk is very nutritious, essential for a healthy diet.",
        "I can't tell if you are joking or you really believe this crap. Point is, I am not drinking it.",
        "How do you expect to become leader of Mandalore with that attitude? Aren't good leaders charming and charismatic?",
        "My sister was a nice and charming leader. Her dream was for Mandalore to become a peaceful and egalitarian nation. That killed her.",
        "Damn, that got dark.",
        "Mandalore does not need a compassionate and gentle ruler. It has always been a civilization of warriors. It must be ruled by a strong and unyielding hand.",
    };

    private List<bool> Bo_Katan_3_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_3_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_3_2 = new List<string>()
    {
        "Say Bo-katan, how was your relationship with your sister? It seems to me you didn't agree much with her.",
        "We were never very close. We had different views on how the nation should be ruled, about our people's future.",
        "To break with all this devastation, she envisioned a peaceful and fair society that would make peace with the planet.",
        "Sounds nice. But I guess that was a bad time to be an idealist.",
        "Exactly, that was my position. Moreover, that was incompatible with our culture and traditions... ",
        "When she became ruler, I was still young and wanted to rebel, along with my Nite Owls, ",
        "I joined an organization called the Death Watch, which planned to bring back Mandalore to its former way of life.",
        "So, you went against your sister. What was the Death Watch exactly?",
        "At that time, I thought we were doing something good, but now I see we were just a bunch of terrorists. ",
        "With the help of a powerful Sith Lord, we seized power from my sister and our leader Pre Vizsla became the new Ruler.",
        "From the stories I've heard, it was better to stay as far away as possible from Sith Lords.",
        "In this, you are not wrong.",
    };

    private List<bool> Bo_Katan_3_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_3_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_3_3 = new List<string>()
    {
        "So, what happened with that Sith Lord?",
        "His name was Darth Maul. A powerful creature filled with rage and hatred, but with a very cunning mind.",
        "We thought we were using him... We were wrong. You cannot toy with a Sith Lord. ",
        "He killed Vizsla and my sister. Then, he appointed a puppet leader and governed Mandalore from within the shadows.",
        "I'm glad there are no more of those. I have enough with my Rancors and Gideons. Did you beat Maul in the end?",
        "I did not stand a chance; it was only thanks to the help of the armies from the republic and Ahsoka Tano herself that we reclaimed Mandalore.",
        "She defeated Maul. Then I became regent for a short period of time.",
        "I briefly faced against Ahsoka once, is she really that skilled?",
        "If you are still alive, it is because she did not aim to kill you.",
    };

    private List<bool> Bo_Katan_3_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_3_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_4_1 = new List<string>()
    {
        "You are doing it on purpose, aren't you? I give up, maybe if I give it to the bartender he will exchange it for something potable.",
        "Come on, I know you secretly like it. What can you tell me about the Jedi? During republic times there were a few of them around, right? You must have encountered some.",
        "Ahsoka could tell you more than me about them. She was once one. During the clone wars, Mandalore was a neutral state, so they tried to avoid us as much as possible. ",
        "However, there was one who became really close with my sister, his name was Kenobi or something like that.",
        "Do you think it is a good life for someone like Grogu? I mean... Look at him, they were supposed to fight against the toughest foes in the galaxy, he looks completely harmless. " +
        "I don't know if he should take that path, it seems too dangerous.",
        "The first time I met Ahsoka she looked like an innocent little girl, but when I fought her, she gave me a round for my money.",
        "Did she beat you?",
        "I would not go so far as to say that...",
        "I see... So she beat you.",
        "...",
    };

    private List<bool> Bo_Katan_4_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_4_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
    };

    private List<string> Bo_Katan_4_2 = new List<string>()
    {
        "Your stories of Mandalore intrigue me, they are so different from what I was told. Tell me more about the culture and society.",
        "Mandalore has always been divided by clans. I am, for example, from clan Kryze, and there are others such as Wren, Saxon, Vizsla...",
        "I remember one of the members in the Tribe was called Vizsla, not a very friendly fellow I should say.",
        "Well, Vizslas were known to be rather cold and ambitious. They were one of the clans that wished to return to the old ways and consequently were exiled to Concordia, one of Mandalore's moons, I sided with them during my sister's reign.",
        "So, the Mandalorian people were not very united?",
        "In politics each clan had their best interests in mind and tended to work pretty often against each other, this caused quite a few civil wars. But when going to battle against an external threat this was entirely different. ",
        "We worked as a unit; all our differences were gone and we became an unstoppable force. That is why I supported our warring culture.",
    };

    private List<bool> Bo_Katan_4_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Bo_Katan_4_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_4_3 = new List<string>()
    {
        "How was it to go to battle alongside the whole Mandalorian army? I guess it had to feel amazing.",
        "Indeed, it felt incredible... I remember people coming out of their houses, standing on the wide streets of Sundari, looking up to see thousands of flying warriors in their shining beskar armours. ",
        "Real mandalorian soldiers glittering in the sky, like shooting stars under a bright full moon night, it was truly a sight to behold...",
        "I would like to have seen it at least once in my life. Now I feel like I want to help you restore Mandalore to that former glory.",
        "I would very much appreciate your help. From what I have seen, you fight as well as the most veteran Mandalorian soldiers. And believe me, if seeing the army was impressive, flying with it felt even greater.",
        "I can imagine it. But I must be careful; your compliments might make me blush.",
        "Sure, make fun of it if you want, and you can turn yourself blue for what I care. Since you never take your helmet off, I will never have to see your ugly face, and I am sure you have not even seen yourself in a mirror since you were a child.",
        "How do you know I'm ugly if you've never seen me either. I think I'm actually quite handsome... Probably.",
        "Sure.",
    };

    private List<bool> Bo_Katan_4_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_4_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_5_1 = new List<string>()
    {
        "How about we make a deal: I give you this and you promise to never ever, for the rest of your life, give me more of this horrible drink.",
        "Wow, are you sure you want to give me that? Thanks, I think we have a deal.",
        "I have never been so happy in my life.",
    };

    private List<bool> Bo_Katan_5_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_5_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_5_2 = new List<string>()
    {
        "You know, after hearing all your stories of Mandalore during this time, I've decided I will help you in your mission. I don't know if it is the right thing to do but it is what I want to do. After calling myself a mandalorian for so long, I think this is the way.",
        "I am glad to hear that. Thank you for honouring your word. Will you not get in trouble with your friends in the Children of the Watch?",
        "Maybe, but I will explain it to them, I'm sure they will understand. I also did not say I would become one of you, I'm still a member of the Tribe, I'll be eternally indebted to them.",
        "I am not so sure they will like it, but this is for you to decide. I hope one day you change your mind. There will always be a place for you among us.",
    };

    private List<bool> Bo_Katan_5_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Bo_Katan_5_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_SERIOUS,
    };

    private List<string> Bo_Katan_5_3 = new List<string>()
    {
        "So, what happens now?",
        "I will leave and make the necessary arrangements. I took Mandalore once by force, but I did not succeed in keeping it for very long. The second time I was humiliatingly defeated by the empire. I do not want to make the same mistakes.",
        "What will you change?",
        "This time I will not come to seize power as an aspirant to the throne. I will come as a rightful ruler and I will be accompanied by mandalorians. Beskar armours will step onto the sands of Mandalore once again, and we will show the galaxy we are still alive.",
        "Nice speech. Did you practice it beforehand in front of a mirror?",
        "You are impossible. Why did you have to spoil the moment? Ahhh, whatever, I will contact you when everything is ready. See you soon Mandalorian.",
        "So long Kryze... I also have a bad feeling about this Grogu, but she doesn't seem so bad. I'll trust her... For now.",

    };

    private List<bool> Bo_Katan_5_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Bo_Katan_5_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_BOKATAN_ANGRY,
        DialogImages.IMG_MANDO_HAPPY,
    };

    //////////////////////////////////////////////////////////////////////////////////////// GROGU ////////////////////////////////////////////////////////////////////////////////////////
    private List<string> Grogu_0_1 = new List<string>()
    {
        "Hey kid, how are you doing? We've been quite busy lately, haven't we? Almost no time to slow down and have a chat.",
        "(Little Grogu looks at Mando perplexed, for he has yet to entangle the mysteries of the spoken language.)",
        "Of course... For some reason I tend to forget you can't talk.",
        "(An innocent and pure smile is outlined in the thin, green, almost non-existent lips of the child.)",
    };

    private List<bool> Grogu_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
    };

    private List<DialogImages> Grogu_0_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_LAUGHING,
    };

    private List<string> Grogu_0_2 = new List<string>()
    {
        "Anything new you want to tell me about?",
        "(The adorable creature extends his arm while opening and closing his three short fingers, he definitely wants something, but his hand can only grasp thin air.)",
        "Sorry kid, I don't have your ball on me right now. It's in the ship. And before you ask me, the answer is no; I'm not going back there to get it for you.",
        "(The long and pointy ears of the child start lowering down, a small cry of sadness escapes his little mouse.)",
        "I told you I'm not going back for a stupid little ball. Next time, remind me about it before we leave the ship. If you are so sad, you can go and get it yourself.",
        "(The kid starts sitting up. It seems like, surprisingly he has understood what Din just said.)",
        "Argh... Ok you win.Here you go... Yes, I know I lied to you.",
        " I just didn't want you dropping it somewhere, because then it is me who has to go looking around everywhere for the darn ball. So, don't drop it, or next time there will be no ball.",
        "(Little Grogu, who appeared to be listening carefully to what Mando was saying, in reality has not understood a single thing).",
        "(Therefore, he gives his big silver friend an adorable smile to whom no one in the galaxy could say no, and joyfully takes the ball in his hands.)",
    };

    private List<bool> Grogu_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
        false,
        false,
    };

    private List<DialogImages> Grogu_0_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_GROGU_LAUGHING,
    };

    private List<string> Grogu_1_1 = new List<string>()
    {
        "Here's a little present for you kid. You better drink this, it will help you grow big and strong, like me. Drink it slowly.",
        "(With a mixture of curiosity and excitement, the little one takes the Bantha milk and starts drinking it). ",
        "(With just a couple of gulps he is finished. One wouldn't be able to say whether there is more milk inside his belly or onto himself.)",
        "Well, I'm definitely not cleaning you up. Next time drink like a normal 'whatever your species is' being. And you are not boarding my ship like this either.",
        "(The charming youngling tries, futilely, to wipe the mess he has made of himself with his small hands. He articulates babbles of frustration, and then looks at Mando with sparkling eyes.)",
        "No, no, and no. I told you to drink it slowly, and you didn't listen to me. Come on, you're like fifty years old! When I was your age... Anyhow, you should have learned how to behave yourself by now.",
        "(The adorable infant starts taking his tunic off. It seems like he pretends to use it to clean himself up and then throw it away.)",
        "What do you think you are doing? There are a lot of people here watching. Put your tunic on again, they don't want to see a 50-year-old child naked... Fine, I'll let you on the ship. ",
        "But let this be the last time you do something like this, otherwise I'll put you in charge of cleaning the Crest for an entire week",
    };

    private List<bool> Grogu_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
    };

    private List<DialogImages> Grogu_1_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_MANDO_ANGRY,
    };

    private List<string> Grogu_1_2 = new List<string>()
    {
        "How did you do it before I found you? I really doubt those Nikto mercenaries cared to tend to all your urges and whims.",
        "(Mando's words are lost in the air. Little Grogu is too busy trying to shoo a big tatooinian fly that has camped on his head and is enjoying a feast of delicious Bantha milk.)",
        "Sometimes I have the feeling I'm talking to a wall... I guess they didn't have much of a choice. You were probably too relentless for them.",
        "(At last, Grogu has been able to scare the fly that had him so concerned. He lets out a sigh of relief and closes his eyes. The thrilling battle against this daunting enemy has left him too exhausted and needs some time to rest.)",
        "Sorry, am I boring you? Fine, I won't bother you anymore. Next time you want some attention though, try with this interesting fly you've just befriended.",
        "(The kid, who had apparently fallen asleep, suddenly opens his eyes and stares at Mando while frowning his wrinkled forehead and uttering a displeased cry of protest. Moreover, he waves his tiny arms in the air in sign of consternation.)",
        "Attaboy.",
    };

    private List<bool> Grogu_1_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Grogu_1_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Grogu_2_1 = new List<string>()
    {
        "Here you have, but if you make another mess, I'm leaving you in Tatooine, I promise.",
        "(Carefully, Grogu grabs the milk and starts drinking it. He succeeds in not spilling any drop. However he cannot avoid getting a blue moustache that makes him look even cuter than before, if this is even possible)",
        "Congratulations, you've learned to drink like a normal person. I'm proud of you.",
        "(The youngling smiles at Mando with his blue moustache. The whole canteen is looking at him like a child would look at a piece of candy. Grogu is enjoying the attention he's getting and starts waving at everyone while making happy noises.)",
        "Stop moving and come here, I need to wipe this thing off of your lips, you are making a spectacle out of us. Maybe instead of a Jedi knight you should become a little clown, that would certainly make it easier for me.",
        "No more danger and, without a doubt, a lot of credits.",
    };

    private List<bool> Grogu_2_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        true,
    };

    private List<DialogImages> Grogu_2_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Grogu_2_2 = new List<string>()
    {
        "There is one thing that bothers me, kid. How come that, after all we've been through, you haven't even said a real word to me, and the first time we meet Ahsoka Tano, you tell her all your life story you little traitor. ",
        "It's very unfair, she doesn't even have ears...",
        "(Little Grogu closes his unproportionally big eyes and attempts to concentrate, then he puts his small hands on his head. Now it seems he is really making an effort. After a few seconds of nothing happening, he gives up).",
        "(He lays down on his seat and lets out a sigh).",
        "What was that supposed to mean? Were you trying to communicate with me? If so, you gave up rather quickly. Here I've been trying for the past few months and you give up after a couple of seconds. Thank you for your enormous efforts.",
        "(The mischievous little one, waves one fist in the air and touches his face with the other hand while mumbling something incomprehensible).",
        "(However, any person with a minimum of experience interacting with other intelligent beings could have guessed that it certainly was not something nice).",
        "Ok, ok, no need to get angry. Is it because of the helmet? Sorry but I can't take it off. I can't do anything about it. By the way... watch your mouth kid.",
    };

    private List<bool> Grogu_2_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        true,
        false,
        false,
        true,
        false,
        false,
        true,
    };

    private List<DialogImages> Grogu_2_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Grogu_3_1 = new List<string>()
    {
        "Like the last time, huh? Taking the enemy's stronghold is the easy part, keeping it is what's complicated, but I'm sure I can count on you.",
        "(The adorable little creature takes the Blue milk and drinks until the very last drop. He drinks the whole thing in one gulp. Inexplicably he has successfully completed the operation without making a mess.)",
        "Good, you'll make a fine Jedi if you follow this path. Do you really want to become one though? I'm not going to force you to do anything you don't want.",
        "(Mando is faced against an impassable visage. Behind that innocent and cute face, lies a potential that not even the very own little one can begin to fathom. And yet, it seems that he understands it in some strange way.)",
        "I know... That's your duty. Especially to the ones that saved your life while sentencing theirs. But how did they do it? It's a miracle they didn't discover you.",
        "(Grogu lowers his head and his ears. Clearly something has troubled his mind and a sad whisper escapes his fragile lungs.)",
        "I understand if you don't want to talk about it. It's ok kid. We are all running away from our past in some way or another",
    };

    private List<bool> Grogu_3_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Grogu_3_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Grogu_3_2 = new List<string>()
    {
        "I'm not sure if going with Bo-Katan is a good idea. She seems to really care about her home and people but there's something that seems off. What do you think kid?",
        "(Right now, Grogu is too concerned with his metallic ball. He has put it in his mouth and is trying to chew it. He then discovers that apparently his species is unable to chew metal, which comes to him as a surprise to be sure, but an unwelcome one).",
        "(Therefore, frustrated, he spits the ball).",
        "Oh great. Now, every time I touch the dang ball on the lever, I'll remember the coat of saliva you so kindly gave it.",
        "(The clueless child utters a joyful cry. It seems like he perchance thinks that he has just been complimented.)",
        "Sure, sure, very well done. Thank you very much for your service. Well... I'll take this as your way of telling me to help Bo-Katan. I will trust you, but then it will be your fault if she turns out to be no good.",
        "(The kid has put the ball in his mouth again. It appears he has not given up on the potential of his tiny jaw, and is going to try to chew the ball once more. He miserably fails in this endeavour; the ball has defeated him.)",
    };

    private List<bool> Grogu_3_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Grogu_3_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GROGU_CURIOUS,
    };

    private List<string> Grogu_4_1 = new List<string>()
    {
        "Another one for you kid. You already know the deal. Nice and easy all right?",
        "(Without wasting a single second, little Grogu drinks the milk. Afterwards he seems somewhat uneasy, could it have been the milk? He lies down to rest.)",
        "It's nice to be able to rest. I know we've had it rough these past months. However, you've proven your value. The truth is, I always wonder what would have happened to you if I hadn't shown up, but I also don't know what would have become of me without you. ",
        "You've saved me too, even if I don't like to admit it...",
        "(The child is sleeping safe and sound. Nobody would be able to guess that in a couple hundred years, this youngling might become the most powerful being in the galaxy. Right now, it looks like he can't even hurt a fly.)",
        "Have a nice sleep kid, you've earned it. And don't worry, as long as I'm beside you, nobody will lay a hand on you.",
    };

    private List<bool> Grogu_4_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        true,
        false,
        true,
    };

    private List<DialogImages> Grogu_4_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Grogu_4_2 = new List<string>()
    {
        "Say kid, did you really have some training back in the Jedi days? From what Ahsoka told me that's what I understood, but I don't get how they were able to teach you anything, considering the troubles I have now trying to teach you to just drink normally...",
        "(After a short, delightful nap, the kid opens his eyes. With renewed energy starts playing with his ball. Now it seems that his new challenge is to get it to fit into his nose).",
        "You have probably forgotten all about it. I understand. All your friends during that time, all your teachers, everyone... they are probably dead. And yet they were able to save you.",
        "(Suddenly, little Grogu raises the ball and with the force he drives it to Mando. He looks at his caretaker with confidence and then demands the ball back.)",
        "You are full of surprises, kid. Catch it! I need to learn not to underestimate you. You are stronger than you seem.",
    };

    private List<bool> Grogu_4_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Grogu_4_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Grogu_5_1 = new List<string>()
    {
        "(The kid, who is not baby Yoda but looks like baby Yoda, drinks once again all the milk in one gulp. Now something is going wrong for real. His green skin starts to turn blue).",
        "(He holds on until he can no longer bear it and throws up everything, making a mess once again, and thanks to the smell, this time a stinky mess).",
        "Why? You were doing so well... That's because you drink too fast. I told you from the beginning; slow-ly. Now you've really done it kid... What's this?",
        "(Among all the nasty visual inventory of the things Grogu had eaten during the last 24 hours, there is a little chip. Mando grabs it.)",
        "Well, you never know... What am I going to do with you now? If you don't like blue milk you could have told me from the beginning.",
        "(Little Grogu's pointy ears start angling down, a tiny tear rounds down his cheek from one of his big eyes. He doesn't even try to wipe himself up this time, he knows it is pointless.)",
        "Oh, stop it. Please, don't cry kid, come on, don't cry. Do you want your ball? Look, here is your ball, grab it and eat it or whatever you want but don't do this to me... Ok, next time I'm going to buy you an ice-cream, no two ice-creams. ",
        "Have you ever tried an ice-cream? Kids love this stuff, and there are ice-creams with all the tastes in the galaxy, you're going to love it... Ugh, I'm not good at this.",
    };

    private List<bool> Grogu_5_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        true,
    };

    private List<DialogImages> Grogu_5_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GROGU_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_SCARED,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Grogu_5_2 = new List<string>()
    {
        "Tell me kid, when all this is over... us I mean. When we find you this wise master... Am I ever going to see you again? ... Another fifty years from now, will you remember an old and grumpy geezer the face of whom you never saw?",
        "(Grogu reaches to Mando with his arms, but they are too short. Mando gets close and the kid puts one hand on the helmet, then he closes his big eyes. A warm funny sensation tickles Mando's face). ",
        "(Then The kid opens his eyes again and smiles at his tall tin-friend while bubbling some sounds beyond comprehension).",
        "I don't know what you just did but it feels like... like you've just been in my head. And in some weird way, I feel at ease. I haven't even found you a master yet and you can already do things I can't understand... You know, I'm going to be sad when you are gone.",
        "(The youngling tilts his head and raises his ears. Then he returns the ball to Mando, who grabs it, looks at it, and gives it back to the child. Behind Grogu's black eyes, one can only but see a deep, daunting void). ",
        "(However, in this very moment, Mando could swear that he is having a glimpse into the entire universe.)",
        "Thank you, but you keep it, there's very little I can give you, so at least keep this. Oh, don't act all tough and strong all of a sudden, I know you'll be sad too. Just pay me a visit whenever your master gives you a little bit of free time.",
        "And if he doesn't let you, I will personally come wherever you are and beat him up. Jedis are no match for me, but don't tell Ahsoka what I just said... You already know this, but there will always be a place for you on my ship.",
    };

    private List<bool> Grogu_5_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        false,
        true,
        true,
    };

    private List<DialogImages> Grogu_5_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GROGU_CURIOUS,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_GROGU_LAUGHING,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_MANDO_HAPPY,
    };
    //////////////////////////////////////////////////////////////////////////////////////// GREEF KARGA ////////////////////////////////////////////////////////////////////////////////////////

    private List<string> Greef_0_1 = new List<string>()
    {
        "Ahh, if it isn't the great Mando himself. How is my best bounty hunter doing these days?",
        "I no longer work for you Greef. I'm doing fine, thanks.",
        "Isn't this nice? All of us, together again, here sharing old memories... I've been rather busy these past months, but now, I'm feeling much better.",
        "What have you been up to? Don't tell me you've got meddled up in business with another nasty client.",
        "On the contrary my friend. I've been trying to make Nevarro a better place. I find your lack of trust in me quite painful. ",
        "Now that I'm the magistrate once again, and with Cara's help, we are going to turn that hole into something decent.",
        "Ah yes, I remember you were trying to do something like that. And since you turned your canteen into a school, now you have to go to other planets to get boozed-up.",
        "One can still enjoy a drink in Nevarro, but places like this one are often a den for ruffians. We've just made it safer for kids and stuff. " ,
        "Pay us a visit next time you fly close to the planet. You'll be surprised to see how it is changed.",
    };

    private List<bool> Greef_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_0_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_LAUGHING,
    };

    private List<string> Greef_0_2 = new List<string>()
    {
        "I bet you are enjoying your new position as magistrate Greef. You've always liked to order people around.",
        "You're right, I like this job, but I prefer to think I got back what was mine. " ,
        "You know I'd already been the magistrate for a long time before those nerf herders from the New Republic came to power and unfairly fired me.",
        "Sure, sure... Because you wouldn't happen to be running some underground business of questionable ethicality, which included illegal trafficking, assassination contracts...",
        "Hey, as far as I can remember, you and your Mandalorian friends greatly benefited from such contracts.",
        "I didn't kill anyone who didn't deserve it. And I usually brought my targets alive.",
        "Tell yourself that if it makes you sleep any better. We did what we had to do to survive, and if it hadn't been me, someone else would have taken my place. ",
        "It is just how it is. Under the rule of the Empire, this was the way. ",
        "But well, as you can see, this has changed now.",
    };

    private List<bool> Greef_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
        false,
    };

    private List<DialogImages> Greef_0_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
    };

    private List<string> Greef_0_3 = new List<string>()
    {
        "There's one thing that still doesn't convince me. What you are doing in Nevarro... do you really believe in it? Or are you just doing it to keep your new job?",
        "You genuinely think that low of me... Look, I could have just cleaned the place up a bit and buried all the bad stuff, because it doesn't matter. ",
        "Nobody is going to look twice to a planet like Nevarro.",
        "But I'm honestly trying. I'm really making an effort to change things. I want people to know that they can safely grow their kids there, and live comfortable lives.",
        "Alright, I believe you. Now that Cara is helping you, I think you might actually be able to achieve what you're saying. It's not that I don't trust you, but I know you're only loyal to yourself.",
        "My days of messing around with wrongdoers are over. It is painful to admit it, but one has already a certain age here. What I want now is peace and tranquillity.",
        "It is quite amusing to imagine the infamous Greef Karga as an old gramps, in a park, seated on a bench while watching the children play.",
        "Very funny. Be careful with what you say. I might not be in my golden years, but I still have a lot in me. I'm still an agent of the Guild and he who underestimates me will find himself in a bad spot. ",
        "But well, you just watch Mando, next time you come to Nevarro you won't be able to recognize it.",
    };

    private List<bool> Greef_0_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_0_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
    };

    private List<string> Greef_1_1 = new List<string>()
    {
        "Oh, is this your way of showing how much you appreciate my friendship? You're welcome Mando.",
        "Don't get too full of yourself. I had one to spare. I can give it to someone else.",
        "No, I appreciate this, and I'll take it. Say, have you reconsidered my proposition? There's still a place for you in the Guild if you want. Since you left, I can't cash any of the big boy bounties. ",
        "The other bounty hunters are not as good as you, except for maybe Cara, but she's too busy being Marshall and has no time for chasing miscreants around the galaxy.",
        "The kid is my mission now. As long as I have to take care of him, it's better to avoid all conflicts. Therefore no, I can't return to the Guild right now. You'll have to settle for the small fish.",
        "Well, maybe when you have delivered the kid to whoever it is that's supposed to take him, you'll reconsider. You've been a bounty hunter all your life and you don't know anything else, once the kid is gone, I know you'll come back.",
        "You don't know what I will do. Maybe I'll come back, maybe I won't. Right now, there's the whole Mandalore affair we'll have to take care of, so I'll be busy for quite a while.",
    };

    private List<bool> Greef_1_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_1_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
    };

    private List<string> Greef_1_2 = new List<string>()
    {
        "Speaking of Mandalore. Have you considered joining us? Bo-Katan has been planning it for a long time, and Ahsoka Tano will help. Cara will probably join too. It will be an interesting experience to say the least.",
        "Why should I come, next to all you monsters, I will be of little help.",
        "Come on, it's not like you aren't a terrific gunslinger too. You alone can take a dozen of bucketheads on your own. And you are an agent of the Guild. I'm sure you can get a few fellow hunters to help with our cause.",
        "Well, if you put it like that, it sure is tempting. I like you throwing me compliments. You should do it more often. But what's in it for me?",
        "Is it not enough to go on a new adventure with your friends? And aren't you curious to know what we'll find there?",
        "I'm glad to see that you finally recognize me as your friend, especially when you are trying to get something from me... ",
        "I don't know if I need any more adventures... I've already seen a lot of action in my life, and age is unforgiving you know... My knees are not what they used to be, my hips hurt, and my sight is--",
        "Alright you old geezer, I get it. I'm sure that if you ask her, Bo-Katan would be willing to reward you with a reasonable amount of credits for your service. After all, she is going to become the ruler of Mandalore.",
        "I don't know how you did it, but I think you have convinced me. If this Bo-Katan is kind enough to give us a little reward as you say, you can count me in. It is strange, but my knees and my hips are feeling so much better now...",
    };

    private List<bool> Greef_1_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Greef_1_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_LAUGHING,
    };

    private List<string> Greef_1_3 = new List<string>()
    {
        "Tell me Mando, what is this Mandalore campaign all about?",
        "The thing is, we don't really know. The objective is to take the planet, so that Bo-Katan can restore Mandalore to the nation it used to be before the Empire decimated it. ",
        "But how we are going to take it is kind of a mystery, because we don't know what is waiting for us down there.",
        "So, we are going to go there completely blind. We don't know who or what we are going to face, and how many of them there will be.",
        "More or less, that's it.",
        "Sounds like my kind of weekend.",
        "Bo-Katan hasn't told me what her plan is. Though from what I have gathered, I don't think we are just going to land there and hope we don't get instantly killed. ",
        "She must have some friends from before they had to leave the planet, some real Mandalorian warriors I would expect.",
        "I'm going to join you, but let me say, I have a bad feeling about this.",
    };

    private List<bool> Greef_1_3_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        true,
        false,
        true,
        false,
        true,
        true,
        false,
    };

    private List<DialogImages> Greef_1_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_2_1 = new List<string>()
    {
        "I suppose I shouldn't take this as a token of our beautiful friendship either, should I?",
        "No, you shouldn't. I had another one to spare.",
        "I saved your butt many times Mando, and you have helped me a lot. No other bounty hunter has ever made me earn so much money. Hasn't our partnership been mutually beneficial.",
        "Yes, it has. But that's what it is, a strategic partnership. You betrayed me not too long ago, remember?",
        "No, it was longer ago. And shall I remind you that you broke a contract? This goes against the Guild's rules, I had no other choice.",
        "I didn't break the contract, I delivered the child as I was expected to do. Afterwards I got it back.",
        "Details, details... Such petty details don't matter to the Guild. And you almost killed me then, remember? Anyway, whatever happened is in the past. Later, I helped you many times. You know I appreciate you. Losing my best bounty hunter is not something I would want.",
    };

    private List<bool> Greef_2_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Greef_2_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_SERIOUS,
    };

    private List<string> Greef_2_2 = new List<string>()
    {
        "You did help me a lot, I have to admit it. And that day, against all the troopers and Moff Gideon, you almost got killed for getting involved with me. ",
        "We're lucky IG saved the day, I'll never come to like droids, but if not for him... Well, we wouldn't be talking right now.",
        "I know you don't hate me Mando... Out of all the group, we two go back the longest. I'm the one who knows you best. By the way, speaking about IG. I tried to rebuild him.",
        "What? But how? He blew himself up. I'd be surprised if there was even a single screw left of him.",
        "You know I'm resourceful. With the help of a few members of the Guild, we tried to find as many parts as possible. After five days we found enough. Turns out his memory drive was kept in a small and very hard metal container.",
        "It was not beskar, but was hard enough to withstand the explosion. We used some of the pieces we had found with new ones and we built him a new body.",
        "Why would you do that? You went through a lot of work for just a droid. If you were in need, buying a new one would have been cheaper.",
        "Hey, that IG saved my life too, plus was a fellow member of the Guild. I kind of felt I owed it to him. I thought he would be useful too, so I sent him to help in several businesses in Nevarro... it was not a good idea. He almost trashed a whole village.",
        "What were you expecting, he was made for combat.. You know I appreciate you. Losing my best bounty hunter is not something I would want.",
        "I know. That's why I directed him towards you. Maybe he can help you in some way or another. So, don't be surprised if one of these days you encounter him.",
        "Why does this have to happen to me? ...",
    };

    private List<bool> Greef_2_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_2_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Greef_2_3 = new List<string>()
    {
        "You never told me what happened after we destroyed that imperial base in Nevarro.",
        "Ah, nothing worth mentioning. I reported it to some high charges in the New Republic, they came to take a look and then they left. Afterwards, we got the licence to join the trade network in Nevarro's sector. This has brought a few investors, which is nice.",
        "Looks like it ended up being profitable for you. For me, discovering that Moff Gideon was alive was quite the opposite, a pretty unpleasant surprise I must say. ",
        "Didn't they tell you what was all that genetic stuff about? That was not a normal base, it's purpose must have been scientific research or something like that...",
        "We didn't leave much to be investigated there. Either way, they couldn't tell me anything. But I think it's for the best. They were doing something scary, and the kid was part of their plans in some way. ",
        "It was right for you to go after Moff Gideon. Otherwise, the kid would still be in danger.",
        "I doubt that was their only base, they had a lot of resources and staff, if all that stuff was so important for the imperials, I bet they had more labs just in case.",
        " Going after Moff Gideon was my only choice, I had to go to the root of all this and end it once and for all. Although, something tells me that it is not yet over.",
        "What makes you say that?",
        " I don't know. But it seems to me that Moff Gideon was not the brains of all this. He had to be working under someone we still don't know. Someone even more dangerous...",
    };

    private List<bool> Greef_2_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        true,
        false,
        false,
        true,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_2_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
    };

    private List<string> Greef_3_1 = new List<string>()
    {
        "I know, I know... you had another one to spare.",
        "Nah, this time it actually is a token of appreciation. Sure, we've had our differences, but it's true that you've helped me a lot.",
        "Wow, where did you get this new empathy of yours? It's unlike you.",
        "Don't make me regret saying those words.",
        "Fine, fine, I won't complain. You know, I used to be like you when I was younger... without the whole Mandalorian thing, of course... I mean I was a bounty hunter too. I preferred to work alone and kept most to myself. ",
        "My aim was to become the best bounty hunter in the galaxy, and I didn't have time for other things.",
        "The best in the galaxy? That's more on the impossible side of things. You are good, no offense, but there are a lot of beings out there who don't care about blasters and tricks. If they want to kill you, you don't stand any chance.",
        "I know, I eventually learned that. But I had an idol. You are too young to have known about him. He was a Duros bounty hunter. ",
        "Like me, he didn't have any psychic powers, superhuman strength or an impenetrable armour. Just two guns, a few tricks up his sleeve and a very cool hat. And despite that, he became the number one bounty hunter during the clone wars era.",
    };

    private List<bool> Greef_3_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_3_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_THREATENING,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_3_2 = new List<string>()
    {
        "It's hard to believe a man like that bounty hunter could thrive in an era filled with Jedi and war. I guess it makes sense that you wanted to be like him. Although it seems like you were missing the cool hat.",
        "Hahaha, I tried some, but I quickly discovered hats are not my thing. I always dropped them, and I had to buy new ones all the time. ",
        "Plus, I have a big head and it was difficult to find hats that--Wait, why am I telling you all this? It is irrelevant to what I was trying to say.",
        "I don't know, it was you who mentioned he wore a cool hat...",
        "Stop trying to make fun of me. Anyway, what was I trying to tell you? ... Ah yes, I remember. I admired that bounty hunter and I thought I could be like him. I trained myself, I improved my accuracy as much as I could, and I got the latest gadgets...",
        "Let me guess. Then you faced some simple goons, they kicked your butt and they made you realize you were not a superhero.",
        "I find your lack of faith in me slightly disturbing, but it's ok, I'm used to it. It was actually rather the opposite. ",
        "I started to get a lot of contracts which I always closed successfully, and eventually, the Empire noticed me. I suppose they thought I could be of use, so I started working for them, one thing led to another and I ended up becoming the magistrate of Nevarro.",
        "I was really not expecting that ending. But hold on, then, what was the point of your story?",
        "I couldn't have become a magistrate if I had just continued to be a lone wolf working for myself.",
        "I didn't become that legendary bounty hunter that I admired, instead I became something better. And for that, I had to make new partners and learn to appreciate them. It is difficult to get anywhere if you are not willing to establish new friendships",
    };

    private List<bool> Greef_3_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_3_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_3_3 = new List<string>()
    {
        "I knew you worked for the empire, but I had never thought about your past with them. I just imagined that they had let you be magistrate as long as you didn't give them any problems. But with all that talk about partners and friendships... ",
        "How deep did you go into their rabbit hole?",
        "I guess that by telling you that story, it was inevitable that you would ask me about this... Look Mando, I'm not proud of a lot of the things I did for the Empire, but I also don't regret doing them.",
        "All those choices I made, in the end, led me to become magistrate of Nevarro.",
        "But how many people had to suffer so that you could get your precious position? The Empire's methods were ruthless, and you became their puppet.",
        "The Empire might have been ruthless and cruel, but this was because of those who ruled it. For people like us, in the lower ranks, we were fighting for a good cause." ,
        "The methods were wrong, no doubt about it, but the ultimate goal was to establish order and peace in the galaxy.",
        "Are you trying to justify what the Empire did? Look at what they did to Bo-Katan's nation, look at Cara's homeplanet Alderaan... In fact, you can't, because they destroyed it.",
        "No, I'm not trying to justify them. What I'm trying to tell you is that there was a dream; after the Empire had total control over the galaxy, there would be no more suffering. ",
        "Ordinary people fought for that dream, but halfway towards it, we started forgetting what it was. And when we realized what we had done and tried to go back, it was too late; the dream was already over.",
    };

    private List<bool> Greef_3_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_3_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_4_1 = new List<string>()
    {
        "What now? Token of appreciation or one to spare?",
        "You are starting to give too much importance to this. Anyway, there's something I want to ask you. You've been around longer than most of us...",
        "If this is your way of telling me that I'm old, nice job; you were successful in making me feel like the grandpa of the band.",
        "You don't need me to tell you that you are old, you can just go to the mirror and look at yourself when you have some time. But this isn't what I was trying to say. I wanted to ask you what you remember about the jedi. You were around when they were still alive.",
        "If I could borrow one from you I would do it, but since you don't take your helmet off to spare us seeing your ugly face, I guess you don't have any mirrors. ",
        "Regarding the Jedi, I can't tell you much, I think I saw one or two from afar, but I never talked with any of them.",
        "If only you were as good with blasters as you are with words... Still, you must have heard stories. From what I understand, Jedis were a big deal back then.",
        "I had no time for stories, I was too busy working and training to become a bounty hunter. Hmm, I remember that Jango Fett, the best bounty hunter during the last years of the Republic, was killed by a Jedi when I was just a kid. ",
        "Maybe that's why later, I tried to stay away from them as much as possible.",
    };

    private List<bool> Greef_4_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Greef_4_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_ASHAMED,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_4_2 = new List<string>()
    {
        "About the Jedi. Remember the other bounty hunter I told you about? The one I used to admire. He became number one after Jango's death. He meddled with Jedi many times and yet, always seemed to come out unscratched.",
        "So, I don't know, maybe they weren't that good, or he was too smart.",
        "It's nice to know that, but it doesn't help me. What I was wondering is whether it is right for Grogu to become a Jedi.",
        "Does he have any other choice? He has the gift. Do you know how much would most people be willing to sacrifice to be able to wield the force? He has a chance to become what most of us can only dream about, and he must learn to defend himself too.",
        "Moff Gideon was not the only one out there who would go to great lengths to get him.",
        "I know all that, but isn't becoming a Jedi even more dangerous? Now, not many people know about his existence. However, being a Jedi would immediately put him on the spot.",
        "You need to trust in him. Jedi were anything but weaklings. Once he is fully trained, it will be those who want to harm him who are going to have to start worrying. He will be stronger than the whole Guild combined.",
    };

    private List<bool> Greef_4_2_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Greef_4_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_SERIOUS,
    };

    private List<string> Greef_4_3 = new List<string>()
    {
        "I briefly fought Ahsoka once. She is extremely good, more than anyone I've ever met, but she is not invincible. A shot from behind, a knife while sleeping, or enough manpower... ",
        "All these things could kill her. If Grogu becomes a Jedi, he will have to keep an eye open during all his life.",
        "Don't we all Mando? You avoid having partners so that they cannot backstab you, and when was the last time you slept without having your blaster next to you and ready to fire? ",
        "Let me tell you, I do the same and I bet so does Cara and even our Mythrol friend. It's just the way it is.",
        "Yes, you're right... Sorry I can't help it. The kid has me worried all the time. What would have happened to him if I had not shown up that day at the Nikto hideout on Aravala-7? He would surely be dead, or serving as guinea pig for that crazy maniac...",
        "You shouldn't think about that, the important thing is that you found him and saved him. Maybe it was fate, have you thought about that?",
        "I don't believe in fate.",
        "Well let me tell you one thing, one can never be sure of anything when getting involved with Jedi stuff. The force works in mysterious ways...",
    };

    private List<bool> Greef_4_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Greef_4_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_GREEF_THINKER,
    };

    private List<string> Greef_5_1 = new List<string>()
    {
        "Now don't tell me that you had that much to spare. I don't care what you say, after five times I'm definitely going to take this as a kind gesture from you, and thus I'm going to do the same. Here you go, take this and use it when you need it.",
        "Thank you, Greef. I certainly will. I think I agree with what you said at the beginning. It's been nice these days. I've learned things I didn't know about you and you've given me some good advice.",
        "You know you can count on me Mando. As long as you don't backstab me again, I'm not going to send all of the Guild after you.",
        "Fair enough. Now that we are past that Gideon problem there's not much we have to worry about. After our Mandalore mission, what are you planning to do?",
        "First, we have to get back from that cursed planet. Something tells me things are not going to go as smooth as Bo-Katan has planned. Afterwards, I will consider what is my next move. ",
        "I'm probably going to keep my position as magistrate, trying to improve things in Nevarro.",
        "I suppose that's what makes most sense for you. Haven't you considered retiring though?",
        "If you try to remind me of my age one more time. I'm going to put a price on your head so high, the whole galaxy is going to be after you.",
    };

    private List<bool> Greef_5_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Greef_5_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GREEF_LAUGHING,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_ASHAMED,
    };

    private List<string> Greef_5_2 = new List<string>()
    {
        "For reclaiming Mandalore, will you ask the members of the Guild for help?",
        "I can ask, but except for a couple of goons who owe me credits, I don't think anyone is going to join if there isn't a reward in the end.",
        "You said Bo-Katan would compensate us, but you can't know if she will actually do it. And even if she did, it can't be a lot, she has probably already spent most of her credits preparing the mission, and she is going to need even more if she wants to rebuild Mandalore.",
        "You're right, I hadn't thought about that. Since the Empire made hell rain over the planet, everything must be in ruins. It will take years to restore the cities to their former glory...",
        "I don't know what you had pictured in your mind. But when we land in the middle of all that decay, the only thing that you are going to see are dunes of earth and sand stretching far away.",
        "I admit that after hearing so much about Mandalore and... Well, believing myself a Mandalorian, I had imagined shining cities of silver hidden inside colossal domes.",
        "Don't be disillusioned Mando. Mandalore might not be that utopia you had dreamed of. But, from the ashes the empire left there, a new civilization will arise, and Mandalore will belong to the mandalorians once again. This is the way.",
        "This is the way.",
    };

    private List<bool> Greef_5_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_5_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
    };

    private List<string> Greef_5_3 = new List<string>()
    {
        "Have you suddenly grown wiser? Lately I have the feeling that what you say is actually worth listening to.",
        "Maybe the problem is that you hadn't listened to me before. When was the last time we talked about anything that wasn't business? You always came with your bounties, dropped a few one-liners, collected your rewards, and immediately left.",
        "I thought I didn't have time to stick around and listen to your banal speeches. I admit I misjudged you.",
        "You may think you have everything figured out, but nothing beats experience, and I assure you, I outrank you a lot in that aspect.",
        "You told me not to mention your old age anymore, now I see why. There is no need for me to do so, you already do it yourself.",
        "Hey Mando, when was the last time we two faced each other in a duel?",
        "Now that's the Greef I'm familiar with. Please, don't ever change.",
    };

    private List<bool> Greef_5_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Greef_5_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_GREEF_THINKER,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_GREEF_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    //////////////////////////////////////////////////////////////////////////////////////// AHSOKA ////////////////////////////////////////////////////////////////////////////////////////

    private List<string> Ahsoka_0_1 = new List<string>()
    {
       "Ahsoka Tano... You are here too? That's unexpected. This canteen is starting to get crowded.",
       "It is nice to meet you again, Mandalorian. I am glad to see that you and Grogu are fine. Since we parted ways, you two had me worried.",
       "There's no need for that, rest assured that I'm perfectly capable of fending both myself and the kid.",
       "I know, and it was not my intention to offend you, I apologize if I did so, you are quite competent. However, going against a Moff is a dangerous endeavour, even for somebody like you.",
       "Well, we survived. I appreciate your concern, but it was unnecessary. What brings you here Jedi?",
       "I have been wanting to discuss with Bo-Katan the whole Mandalore affair. ",
        "I do not know if I could speak of her as a friend, but we go back a long time... I do not want to lose her too. Furthermore, I wanted to check on Grogu.",
    };

    private List<bool> Ahsoka_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_0_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_0_2 = new List<string>()
    {
       "So, have you changed your mind about training the kid? I still need to find him a Jedi master.",
       "I am sorry, my answer is still no. I am not a Jedi, I left the Order a long time ago. And I know that Grogu's place lies not with me.",
       "I don't know any other person who can teach him. Are there really any Jedi left nowadays? Because it doesn't look like it. I have the feeling I'm chasing ghosts.",
       "I am afraid these feelings you have are not without foundation. The Jedi Order disappeared at the end of the clone wars, and most of its survivors died shortly afterwards.",
       "Then it's true. There are no longer any Jedi left...",
       "The Jedi Order might be no more, but for the past years, there has been balance in the force. ",
       "There are some out there who are still following the old Jedi ways. The light side of the force is strong with them. If it is Grogu's fate to be trained, you will find them...",/**/
       "or perchance... they will find you.",
    };

    private List<bool> Ahsoka_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_0_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
    };

    private List<string> Ahsoka_0_3 = new List<string>()
    {
       "Why haven't you tried to find any of these Jedi? The light side is the good side, right? They could prove valuable allies.",
       "As a matter of fact, I have spent these last years searching for one of them in particular. ",
       "He was a friend of mine. Last time we met he saved my life, and before splitting up, I promised I would find him later. However, he vanished.",
       "Maybe I should help you. If we found him, could he be able to train Grogu?",
       "Thanks, but this is something I have to do by myself. At that time Ezra was good, though not a fully-fledged Jedi Knight. " ,
       "I do not know how much he has improved during this time, but he might not be ready to become a master yet.",
       "Great, another dead end, my odds are only getting better... Do you have any lead on where you could find him?",
       "Only one. If I want to find him, I must find another man. He was a grand admiral and the most brilliant strategist the old Galactic Empire ever possessed... He was best known as Thrawn.",
    };

    private List<bool> Ahsoka_0_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_0_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_1_1 = new List<string>()
    {
       "Blue milk... Why are you giving me this Mandalorian? It was one of the most popular beverages among the Jedi, did you know that?",
       "Sure, sure, that's why I'm giving it to you. Enjoy it.",
       "I can sense you are lying.",
       "Hey, don't play mind tricks on me Jedi...",
       "I am not a Jedi, and I do not need to use the force to see through you. You are not a particularly good liar.",
       "Ok, alright... Honestly, I didn't know what to do with it, so I figured maybe you would like it.",
       "It is fine. I appreciate it. In the future, should you offer me more, I will gladly accept it.",
    };

    private List<bool> Ahsoka_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_1_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
    };

    private List<string> Ahsoka_1_2 = new List<string>()
    {
        "You keep saying you're not a Jedi. Still, you fight with lightsabers, you use the force, and you're on the good guys' side. I have never met a Jedi, but from the stories I've heard, you tick all the boxes... Help me understand.",
        "Since I was a child, I was raised by Jedi in the Jedi ways... Eventually, I realized that things are not as black and white as they had taught me. While in The Order, I was so naively blinded by our just cause, that I was unable to see the dark shadows our bright light",
        "had casted. Then I understood; there lies darkness in light, and likewise, one can find light in darkness.",
        "Am I supposed to understand what all this means? Now I'm even more confused.",
        "There is no such thing as simply good and bad people, we are far more complex. I was not sure we were the 'good guys' anymore. What I had once considered my home, became unfamiliar, and where before I had seen friendly faces, I could only see strangers.",
        "It became clear to me that I no longer belonged in The Order.",
        "I'm going to pretend that this was more clarifying than before... You make it sound as if Jedi were bad people. Why should Grogu become one then? It doesn't seem like a good idea anymore.",
        "No, do not get me wrong. The Jedi were definitely trying to do good, it is just... those were hard times, and they had to make a lot of difficult decisions. I do not think it has ever been tougher for the members of the Jedi council.",
        "I even considered rejoining after a while, but then order 66 was issued and... Well, it was too late...",
    };

    private List<bool> Ahsoka_1_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_1_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_1_3 = new List<string>()
    {
        "So, Grogu should become a Jedi. Are you sure this is the right way?",
        "I cannot tell you if it is the right way or not. That, only time can tell. But if he is to be trained in the force, he must have a true Jedi knight for a teacher. Someone who can help him embrace the light side of the force and get rid of all his negative emotions.",
        "Last time we met you mentioned the dark side is strong within him. Is this the reason why he can only be trained by a Jedi?",
        "Yes, he has to learn to let go of his fears. He is too attached to you, and this makes him vulnerable. I have witnessed such feelings consume one of the greatest Jedi knights... and this is something that must be avoided at all cost.",
        "I know I'm not one to talk, but to remove all attachments to other people doesn't sound right.",
        "I am not saying that he cannot have any friends, however the way of a Jedi is a lonely one. If he is willing to walk that path, it is his duty to accept certain responsibilities. It may look cruel or unfair, but he will eventually understand.",
    };

    private List<bool> Ahsoka_1_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_1_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_2_1 = new List<string>()
    {
        "Oh, I did not think you would actually take my petition seriously. Thank you anyway.",
        "I had one to spare... There is one thing that's been bugging me; how long does it take to train a Jedi? From what I understand, Grogu is around 50. I keep thinking of him as a kid, but in reality, could he be too old?",
        "He really is a remarkable being. I have only met one other like him in my life. Perhaps the greatest Jedi master who ever lived. He was more than 800 years old... You need not worry about these things, every species ages at its own rhythm.",
        "He may be 50, but he is still a youngling.",
        "More than 800 years?! By the time I die he'll probably still be too young to even get a speeder licence. I hope at least he'll have learned to talk...",
        "Do not underestimate Grogu. The fact that you cannot understand him does not mean he is unable to communicate with others. I can speak with him through the force for example.",
        "But the kid is so helpless. He'll have to rely on others for his protection for who knows how many years...",
        "I was 14 the first time I went into battle. And you would be surprised how capable force-sensitive kids can be. I used to think like you too, but one day a small group of younglings I was supposed to take care of proved me otherwise.",
    };

    private List<bool> Ahsoka_2_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_2_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_2_2 = new List<string>()
    {
        "Grogu will live more than 800 years... Do you think he'll remember any of us in a couple of centuries?",
        "I am not going to pretend to know how living for such a long time affects someone's mind. Nonetheless, what I can assure you is that Grogu is not just anyone. His kind are very special. ",
        "The old master I told you about always had an answer for every question, and all the Jedi turned to him every time they needed counsel. He was the wisest one in the Jedi council and most of the other members had learned from him.",
        "That doesn't really answer my question. What would have been for him a few years when he was a kid compared to almost a millennium of experience studying the mysteries of the universe?",
        "Memories are powerful, the mind can decide to keep or to erase them based on their nature and the impact they had on you, but they always stay in your subconscious. ",
        "The Jedi have a deeper connection between their mind and their subconscious, for we cannot allow repressed emotions to cloud our judgement and affect our actions. In the same way, we can access old memories that are buried deep in our subconscious.",
        "Does this mean he will remember us?",
        "To answer what is really troubling you... Yes, he will remember you.",
    };

    private List<bool> Ahsoka_2_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_2_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_2_3 = new List<string>()
    {
        "I wanted to ask you Mandalorian, why do you care so much for Grogu? I have encountered a fair amount of people like you in my life.",
        "Bounty hunters with good reputations who prefer to work alone and would stop at nothing to get their mission done, so that they can earn their desired credits at the end... None of them would have broken a deal to save the child.",
        "I suppose I'm not like the other bounty hunters you've met. I just... I felt it was the right thing to do.",
        "That might be what you tell yourself, but I sense there is something more. Something that makes you care for him and has created a strong bond between you two...",
        "I told you I don't like these Jedi tricks. Get out of my mind!",
        "My apologies, but this time I needed to be sure. Even if I do not accept to take him, I must know he is in good hands. I already told you that he is very special. ",
        "Alright, but don't do it again. I guess... Yes. That day, on my homeplanet. I was a child too... alone, under that hatch... I know. He reminds me of myself.",
        "Thank you for telling the truth.",
    };

    private List<bool> Ahsoka_2_3_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_2_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_THREATENING,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_SERIOUS,
    };

    private List<string> Ahsoka_3_1 = new List<string>()
    {
        "Another one to spare? Where do you get all this Bantha milk?",
        "You know... here and there. Doesn't matter. Have you spoken with Bo-Katan about the Mandalore business yet?",
        "I have indeed. I knew she would try to reclaim Mandalore once again, and I agree with her, the time has come. There are still vestiges of The Empire scattered around the galaxy, but they are the weakest they have ever been.",
        "What do you think is looming in there? I was always told that the planet was cursed, but nobody knew exactly what that meant.",
        "I am also clueless here. My guess is that either there is an old Empire stronghold, or it has become a den for fugitives, low-life mobsters, and criminals.",
        "I hope it's the later one, I'm more used to dealing with this type of scoundrels.",
        "Whatever it may be, I am certain that they are not going to just hand over the planet and leave. You should never underestimate the enemy. Bo-Katan has been planning this for a long time. This time, I hope she can return to stay once and for all.",
    };

    private List<bool> Ahsoka_3_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_3_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_3_2 = new List<string>()
    {
        "What went wrong the last time you two tried to claim Mandalore?",
        "Nothing went wrong. It was certainly difficult, and we had a lot of casualties, but we were ultimately successful. You can ask Bo-Katan for the details.",
        "The problem was that the Republic fell hours later, and since she had claimed Mandalore with their aid, the planet was targeted by The Empire. She could not keep her position for long.",
        "Well, you already did it once. I'm sure you can do it again.",
        "Last time I had an entire division of troopers from the Republic with me and Bo-Katan had a lot of Mandalorian warriors with her.",
        "I'm sure that we can do without the boys in white; their only purpose is to add bulk. One Mandalorian counts for twenty of them. We can find the numbers elsewhere.",
        "You do not know what you are talking about, you are too young. Those were not your usual storm troopers; those were clone troopers. They were created specifically to become soldiers in the Grand Army of the Republic.",
        "Fighting was in their blood. I have never met any army as brave and disciplined as them.",
    };

    private List<bool> Ahsoka_3_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_3_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_3_3 = new List<string>()
    {
        "I think you exaggerate a bit. If clones were so good, why did The Empire stop using them?",
        "There was no need for that amount of manpower anymore, they were expensive to produce, and could be programmed to do anything, such as betraying their commanding officers.",
        "That's what happened to the Jedi right?... They put too much trust in them.",
        "That is sadly true... However, I would not have fought with anyone else during the clone wars. When they turned against the Jedi... it was not their fault. They could not help it. Even if it was against their will.",
        "Seems like you were really fond of them.",
        "We went through so much together... They saved my life countless times. Some of them became dear friends. I was able to save one at least, my second in command. ",
        "His name is Rex, the finest soldier I have ever known. He was beside me during our mission in Mandalore. He was beside me from the beginning until the very last moment...",
        "The way you talk about him, I'd have liked to meet this man. He would have been a great addition to our team now.",
        "Oh, you misunderstood me, he is still alive. That geezer, he last saw action during the battle of Endor, despite me advising him not to do it because of his age. ",
        "I am sure if I told him, he would come without hesitation. I am not going to do it though. He has earned a well-deserved retirement. After a life full of war, I want him to spend his last years in peace..",
    };

    private List<bool> Ahsoka_3_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_3_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
    };

    private List<string> Ahsoka_4_1 = new List<string>()
    {
        "You are spoiling me Mandalorian. I am sure other people would profit from this. Consider giving some to them.",
        "I'll give it to whom I want. I have quite a bit so don't worry. After the fall of the Republic, didn't you consider joining the Rebellion? They would have benefited from someone with your skills.",
        "Have you ever heard the word Fulcrum?",
        "Sounds familiar... Wasn't he supposed to be some kind of secret agent working for the Rebellion?",
        "Close, but it was not a he.",
        "It was you? So, you never stopped working for the Republic side.",
        "Several people became Fulcrum during the war, but I was the first one. I directly worked with senator Bail Organa to create what became the Alliance to Restore the Republic, and I was in charge of the Intelligence Service.",
        "Then, you could use your contacts there to get us some help from the New Republic. With their aid, reclaiming Mandalore is going to be no trouble.",
        "I am afraid this is not possible. I stopped working with them when the war ended. And as you said, Fulcrum was supposed to be a secret agent, only a few knew about my identity… No one in the senate would agree to support our cause. We will have to take Mandalore by ourselves. But do not worry, we will not be alone, Bo-Katan has been planning this for a long time.",
    };

    private List<bool> Ahsoka_4_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_4_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_SERIOUS,
    };

    private List<string> Ahsoka_4_2 = new List<string>()
    {
        "It amazes me that you did so much for the Rebellion and still, your involvement has remained a secret. The Republic owes you part of their existence, but your name will be forgotten after you die.",
        "I did not do what I did to get a medal or expecting to become a hero at the end. I did what I felt was right, and to know I helped those who needed it the most, is the greatest reward I can get. ",
        "I know this is not your way, bounty hunter, and yet I think that by now, you might be able to understand it.",
        "Yes, I think I can understand.",
        "I spent the first half of my life fighting for the Republic. During those years, I tasted victory and defeat, glory and condemnation, I was given respect and criticism... All those moments are lost in time, they are meaningless, for few now live who remember them. ",
        "However, when I look in the eyes of the people who suffered under the Empire, who are living peacefully and with freedom today, and I see them sparkling with happiness and joy... I cannot ask for more.",
        "Maybe you aren't a Jedi, and it is too late for you to go back and become one. But from all the stories I've heard, if there's one thing I'm certain about, is that you embody all the great things a Jedi master is supposed to stand for. ",
        "I'm sure that if they were alive now, they would be proud of you...",
        "Thank you Mandalorian. This means a lot to me...",
    };

    private List<bool> Ahsoka_4_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        false,
        true,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_4_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_4_3 = new List<string>()
    {
        "I have realized I owe you an apology Mandalorian. I read you wrong the first time we met.",
        "Well, uh... apology accepted. What do you mean?",
        "The first time, I decided to leave Grogu with you because I thought that, as he was your mission, you would protect him and keep him safe. But now that I know you better, there is no doubt in my mind that it was the right choice. ",
        "He is more than your mission, and you are more than just a bounty hunter.",
        "I'll take that as a compliment, I guess. But hold on a second, are you saying that if you had considered me untrustworthy, you would have taken Grogu? You know I wouldn't have let you, right?",
        "There is no need to think about that, it did not happen. However, there is a chance I might have done it. But do not worry, I would not have harmed you. You treated Grogu well.",
        "You really think I wouldn't stand any chance against you... Your lightsabers can't cut through my beskar armour, and not to brag but I'm a good fighter. You wouldn't have it so easy...",
        "Fine. I remember our small fight the first time we met. If it makes you feel any better, I acknowledge you were surprisingly capable, and fairly resourceful. ",
        "You reminded me of a certain bounty hunter, a blue skinned gunslinger who caused us a lot of trouble when I was still a Padaw--",
        "Alright, ok I get it... Thank you, that definitely made me feel better.",
    };

    private List<bool> Ahsoka_4_3_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
    };

    private List<DialogImages> Ahsoka_4_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Ahsoka_5_1 = new List<string>()
    {
        "Please, let this be the last time you give me blue milk. I am grateful, but I cannot help but wonder whether others might benefit from it more than me. Now, instead, accept this as a gift from me.",
        "Thanks, and ok, I'll do as you ask. I've got to tell you, I know you've already refused many times, but now more than ever, I think you would make a great master. I wouldn't have any complaints if you decided to take the kid with you.",
        "And I must continue to refuse. I am glad that you feel that way about me. But the kid belongs with real Jedi, and I do not see myself as a master. In fact, I do not think I will ever be able to be a master.",
        "But why? I honestly think I'm right here. As a master--",
        "Please... Stop talking about being a master, this brings back painful memories.",
        "Sorry, I didn't know... But please try to see it from my point of view...",
        "I know you think it is right, but there are a lot of things you do not know about me. I am become who I am, as a result of how I was taught, and the person who taught me was the best master I could have ever asked for; still... he was not good enough...",
    };

    private List<bool> Ahsoka_5_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Ahsoka_5_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_SERIOUS,
    };

    private List<string> Ahsoka_5_2 = new List<string>()
    {
        "Can I ask you what happened with your master?",
        "I suppose I should be able to talk about him. What happened belongs to the past, and I need to move on. You see, when I was young, I was quite different from how I am now.",
        "This is true for everyone.",
        "What I mean is, I was not as calm and collected as I am today, and I was very inexperienced. I was eager to prove myself and I often acted hastily and carelessly, without thinking much or considering the consequences of my actions.",
        "Kids tend to be like this, it is not something to be ashamed of.",
        "But I was not just any kid, I was a Jedi Padawan. It was expected from us to be disciplined, focused and obedient to our masters. Any Jedi master would have deemed me a lost cause, any but Skyguy. He taught me a lot, and little by little, I changed into what I am today. ",
        "Everything was going well, until one day everything started to change. I left the Order, and shortly after, he became someone else.",
    };

    private List<bool> Ahsoka_5_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Ahsoka_5_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
    };

    private List<string> Ahsoka_5_3 = new List<string>()
    {
        "What do you mean by that your master became someone else?",
        "Exactly that. I lost track of him, and for a few years I thought he had disappeared. Then, one day, I met him again. He was no longer my master, he was someone... or something else, something vile. ",
        "Where once I had seen kindness, now there was just an overwhelming sense of hollowness and misery... He had taken me under his wing, had looked out and cared for me, and had taught me so much... and I, in the end... I was incapable of helping him...",
        "Now I understand why you said these were painful memories. I'm sorry I asked you about it. Most of us have a dark past, I guess overcoming it and being able to see the light at the end of the tunnel is what makes us be better persons.",
        "You know Mandalorian... I think there is a wise master hidden deep down inside you too.",
        "Oh please, I was being sincere for once... Don't make fun of me.",
        "I was being sincere too. I can rest easy knowing that Grogu is in really good hands, and this makes me happy.",
        "I appreciate that... So now, what's next?",
        "Well, I think now we have a planet to reclaim, don't you agree? It is time for a new episode; meet old friends, make some new ones, get betrayed by them, forgive... and tell the story to the next generation. And most important: may the force be with you.",
        "I couldn't agree more. And, whoever or whatever it is that we will find in Mandalore, they better be prepared, because we are coming for them.",
    };

    private List<bool> Ahsoka_5_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Ahsoka_5_3_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_AHSOKA_MELANCHOLIC,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_AHSOKA_THINKER,
        DialogImages.IMG_MANDO_HAPPY,
    };

    //////////////////////////////////////////////////////////////////////////////////////// CARA DUNE ////////////////////////////////////////////////////////////////////////////////////////

    private List<string> Cara_0_1 = new List<string>()
    {
        "How are you doing Cara?",
        "You know... here, chatting about old times. I feel like an old granny.",
        "What are you talking about? It hasn't even been that long since we met that day in Sorgan.",
        "I guess not, but seems like it was ages ago, doesn't it? We have been through enough trouble; a damn fine novel could be written out of us.",
        "I guess we've had our own fair share of action. You surprise me, I didn't take you for the reader type.",
        "Well, while hiding in Sorgan I had a lot of time to kill. And one needs to entertain herself in-between beating up storm troopers and curious mandalorians, you know.",
        "Sure, dreaming is free.",
    };

    private List<bool> Cara_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Cara_0_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_JOKING,
    };

    private List<string> Cara_0_2 = new List<string>()
    {
        "Do you really think you can beat me?",
        "Why not? I already did once. The first time we met, we didn't have what you would call a very friendly introduction.",
        "Maybe your memory is a little bit rusty. As far as I can remember, that ended in a draw. And I was not even fighting seriously...",
        "That only ended in a draw because of the child. I wouldn't want to beat his daddy in front of him. Also, you were wearing a full suit of armour, that's cheating.",
        "You can make all the excuses you want. We can solve this right now, no blasters, what do you say?",
        "Are you going to take off your armour and fight fair and square?",
        "I can't remove my helmet.",
        "That's what I thought.",
    };

    private List<bool> Cara_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_0_2_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_CARA_SERIOUS,
    };

    private List<string> Cara_0_3 = new List<string>()
    {
        "Seriously, is it that bad to remove your helmet? I promise not to tell anyone.",
        "That's not how it works. The moment someone sees my face, I will stop being a member of the Tribe.",
        "But they don't need to know, it's not like they are watching every step you make right?",
        "I don't think so, but to be part of the Tribe is a choice, and an honour.",
        "I wouldn't have joined if I wasn't entirely sure that this is what I wanted. To always wear a helmet is a small price to pay.",
        "Do you realize this whole Tribe stuff sounds pretty obscure? It's not good to blindly follow rules without ever questioning them. That's why I left the Alliance.",
        "You are starting to sound like Bo-Katan... The Tribe is not like the New Republic, we look for ourselves and swear allegiance to no one. ",
        "I only pick the contracts I like so I don't have to do anything I don't want to do.",
        "I too used to think like you.",
    };

    private List<bool> Cara_0_3_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        true,
        false,
        true,
        true,
        false,
    };

    private List<DialogImages> Cara_0_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_1_1 = new List<string>()
    {
        "Wow, Bantha milk? Thanks! It's been a long time since I drank it. I used to like it a lot when I was younger. ",
        "Then, in the military, we only got field rations and water. That basically killed my taste buds.",
        "Glad you like it. I'll try and bring you more if I can. Was it tough there?",
        "At the beginning it was, when I was fighting in the Alliance to Restore the Republic. " ,
        "We had to fight against the full force of the empire's troops. After winning the war it became a completely different thing.",
        "I suppose it became much easier. Hunting the remnants of the empire doesn't sound as dangerous.",
        "Well, they didn't use to just surrender without putting up a fight, but yes, your odds of coming out alive were much higher.",
        "I don't understand why you decided to resign at that point, the hard work had already been done. Were the new missions not exciting enough for you?",
        "They were exciting alright. But the New Republic Defence Force had more purposes than just hunting leftovers. We also had to escort important delegates and pacify riots.",
        "I had not signed for that. I felt like I wasn't fighting for a cause I believed in anymore.",
    };

    private List<bool> Cara_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Cara_1_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_1_2 = new List<string>()
    {
        "Now you are working for the New Republic again. Have you changed your mind on the nature of its cause?",
        "Hmm... It's not really the same thing. I'm just a Marshall now, it's true that I'm employed by the New Republic, but I don't have to do anything for them. I just make sure there's peace and order in Nevarro and report in case there's any trouble. I like it.",
        "Well, it's nice you've found something you like to do. Is Nevarro starting to feel like home?",
        "My home does not exist anymore, and nothing will ever be able to replace it... But Nevarro is... not too bad, I could get used to it. Life is fine and people are nice.",
        "I've lived most of my life in Nevarro, does this mean you think I'm nice?",
        "Hah... don't push your luck Mandalorian.",
    };

    private List<bool> Cara_1_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_1_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ASHAMED,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_ANGRY2,
    };

    private List<string> Cara_1_3 = new List<string>()
    {
        "Was your homeworld much different from Nevarro?",
        "Alderaan was a beautiful planet. It had amazing landscapes of every color you could imagine. There were advanced cities with the most elaborate architecture that blended harmoniously with nature. ",
        "Its inhabitants were peaceful people who tried to live their lives without conflict and despised war.",
        "But they were one of the supporters of the republic, weren't they? Therefore, they were in war against the Confederacy.",
        "Alderaan had been a member of the Republic since ancient times. They didn't want a war against anybody. During those times of turmoil they tried to make peace and help innocent people as much as possible. And they were targeted for that.",
        "So that's why afterwards, the empire destroyed the planet?",
        "No, they destroyed it just because they could. It's true that Senator Organa, who was from Alderaan, was one of the leaders of the resistance, but they didn't need to destroy the whole planet.",
        " I was offworld when that happened and I lost everyone, that's why I joined the rebellion.",
        "I'm sorry for your loss. Seems that people losing their planets is a recurring theme nowadays...",
    };

    private List<bool> Cara_1_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
    };

    private List<DialogImages> Cara_1_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    private List<string> Cara_2_1 = new List<string>()
    {
        "Another one? Are you trying to get something from me?",
        "Ugh, it hurts that you would think that of me. I'm just giving a present to a friend. Something wrong with that?",
        "Of course not. Friend? It's just, uh... weird, coming from you. You are not exactly the friends-are-the-most-important-thing-in-the-world type of guy.",
        "What kind of guy am I then?",
        "More like the lone wolf type, no offense.",
        "Non taken. Well, the thing is... I don't trust just everyone; it would be foolish. In our line of work, people have a tendency to backstab and double-cross each other for the smallest amount of credits. But I can see the value in having associates and partnerships.",
        "Associates and friends are not the same thing.",
    };

    private List<bool> Cara_2_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_2_1_images = new List<DialogImages>()
    {
		// Mando true, other false
		DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_2_2 = new List<string>()
    {
        "Say, have you been up to something interesting later?",
        "Nothing worth mentioning. You know... saving a village from Klatooinian raiders, making a suicidal last stand against an entire squadron of storm troopers, being chased by a Moff...",
        "I... didn't mean that. But well, at least you were in good company.",
        "I wouldn't have been in those situations if I hadn't been in this 'good company'.",
        "Do you regret getting involved with my things?",
        "Nah, I actually had a good time. It always feels good to beat up some Imps. Especially if they are ex-empire. Although it tends to be even better if there are some credits waiting for you once the job has been done.",
        "You will have to settle for my gratitude and some Bantha milk. That isn't so bad, right?",
        "...",
    };

    private List<bool> Cara_2_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_2_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_2_3 = new List<string>()
    {
        "Look Cara, I promised Bo-Katan I would help her reclaim Mandalore. I'm not sure if I should do it but if I did, would you help us?",
        "You want to drag me into one of your dangerous missions again?",
        "It's not my mission, it's Bo-Katan's, we would be there just to provide support. But yes, chances are it's going to be dangerous. We don't know what the empire left there.",
        "I don't know much about this Bo-Katan. She doesn't strike me as a very trustworthy character. I have the feeling she is hiding something.",
        "Give her a chance, you can't know that if you haven't talked to her. Think about it. If we help her, she will probably reward us, you will get your cherished credits for once.",
        "I'll consider it. But if I help, it will not be for her. I will do it for all the innocents who got slaughtered there. I have a soft spot for people whose planet got decimated by the empire.",
        "I know. Thank you",
    };

    private List<bool> Cara_2_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Cara_2_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Cara_3_1 = new List<string>()
    {
        "Here you go. Now don't complain you don't get anything from me.",
        "Alright, you win. I appreciate your gifts. Still, credits would be better...",
        "You can't drink credits. And blue milk isn't so easy to get, you know.",
        "You should give it to Grogu. He needs it to grow and become a strong jedi. He's so tiny...",
        "I already do, the kid loves blue milk, but if I give him too much, he is going to get stuffed. He doesn't know when to stop. Do you really think he should become a jedi? He's supposed to be over fifty years old, still he's completely helpless.",
        "I'm sure he'll do fine. I remember stories from my time as a shock trooper. They were convinced we would win the war because a jedi knight was fighting alongside us. ",
        "I never got to see this Jedi myself, but apparently, he was the one who defeated Darth Vader and the Emperor at the end of the war. If Grogu turns out to be half as good, there's nothing to be afraid of.",
    };

    private List<bool> Cara_3_1_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Cara_3_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_3_2 = new List<string>()
    {
        "Do you remember who this jedi was? He might be the answer I've been looking for. If he is still alive, maybe he could train the kid.",
        "I haven't heard anything from him since I left the army. After the battle of Endor, he was supposedly still fighting against the remnants of the empire here and there. ",
        "I never found out if he was real or just a legend to keep our morale in high spirits. But if he really existed, he could very well be alive.",
        "That's great! When I met Ahsoka, I thought my mission was over, but she wouldn't take the kid, so I found myself at the starting point once again. Right now, I'll settle for a legend, it is better than having nothing.",
        "Don't get too excited. I never believed it, but... even if he really existed... He could be dead by now.",
        "I don't lose anything for trying. Do you remember what he was called?",
        "Sure, the name of someone like that isn't easily forgotten. His name was... Skywalker.",
    };

    private List<bool> Cara_3_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_3_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
    };

    private List<string> Cara_3_3 = new List<string>()
    {
        "Tell me Cara, do you have any idea of how I can find this Skywalker?",
        "Hmm... Maybe, but it will probably turn out to be a dead end.",
        "Nevermind about that, please tell me.",
        "If the stories they told about the jedi were true, he must have been in contact with the high officers and commanders of the Rebel Alliance, most of whom became leaders of the New Republic afterwards. ",
        "Since in my new job I work for the republic, I can try to find someone up in the chain of command who might have met him.",
        "That would be great, but slow. And I don't think anyone who knows something about him is willing to tell it to a simple Marshall from who knows what planet...",
        "Well thanks, that didn't hurt at all... See, I already told you it was probably not a good idea.",
        "No, I mean... Your idea was good, just a little too, uh... bureaucratic. Why don't you give me a name and I will pay him or her a formal visit myself.",
        "Why do I have a bad feeling about this? ... Alright, but don't mention me. I just got this job, and I would like to keep it for now.",
        "Don't worry, you can trust me. First comes Mandalore though, then we'll have enough time to figure all the bits and pieces",
    };

    private List<bool> Cara_3_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Cara_3_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Cara_4_1 = new List<string>()
    {
        "I think I could get used to this, you know. Sitting here, chatting and drinking, you bringing me gift... After all we've been through, we deserve some peace and quiet.",
        "Yeah, I agree. However, in my book, things don't tend to stay peaceful and quiet for a long time.",
        "Oh please, don't jinx it. I'm sure you'll have plenty of shooting and killing to do in no time. Babysitting a force-sensitive-last-of-its-kind-cute-little creature... trouble is sure to come knocking at your door. No need to search for it.",
        "Poor kid, I wish it were different... He is not going to have it easy in his life.",
        "He seems to be having fun for the time being. And he knows he can count on you. Sure he's going to have to beat up a lot of imps in his life, but he will get used to it. I did, and so did you. He'll just need to up the game a little bit.",
        "I guess you're right. (To Grogu) Someday you're going to be so strong, we'll look like baby porgs next to you. Until that day comes, I'll be here...",
    };

    private List<bool> Cara_4_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<DialogImages> Cara_4_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_HAPPY,
    };

    private List<string> Cara_4_2 = new List<string>()
    {
        "Were you like this in your civilian life in Alderaan?",
        "What do you mean 'like this'?",
        "You know... uh...",
        "Built and strong?",
        "No, yes, strong... One of your punches could K.O. a wookie. It's not something one would expect from an Alderaanian girl.",
        "You were trained by mercenaries; I was trained by soldiers in a period of war. Shock troopers were always the first to engage the enemy in battle. It was not enough to be able to know how to shoot. ",
        "Being so close to the enemy lines, more often than not the fights got physical, and if you want to knock out a stormtrooper in his armour, you've got to hit hard.",
    };

    private List<bool> Cara_4_2_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Cara_4_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_CARA_ANGRY2,
    };

    private List<string> Cara_4_3 = new List<string>()
    {
        "I was also trained in hand-to-hand combat. The members of the Tribe are not only mercenaries, most of them were also warriors of Mandalore back in the day.",
        "But now they're just mercenaries. Besides, your suits don't seem to be very practical for this kind of stuff. ",
        "In the military, we had a training program where we entered a cage and fought one another without any protections or equipment. I never lost a single fight.",
        "The armour is better than it seems, it's pretty flexible. But what was the point of that? Who would go into a fight without equipment?",
        "Forget it... Ah, I just remembered we never finished our arm-wrestling match. That would settle things, right? ",
        "Surely the great and respected Mandalorian wouldn't lose to a 'poor alderaanian girl'. This time though, tell the child he's not allowed to help you.",
        "You are no 'poor alderaanian girl'. And I didn't ask Grogu to help me last time, he did it by himself. I... I don't have time for that right now, maybe another day.",
        "Sure, maybe another day. I think somebody's a sore loser...",
    };

    private List<bool> Cara_4_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        false,
        true,
        false,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_4_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_ANGRY,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_HAPPY,
        DialogImages.IMG_CARA_SERIOUS,
    };

    private List<string> Cara_5_1 = new List<string>()
    {
        "Thanks Mando, I appreciate your kindness, but I must ask you not to give me more of this blue milk.",
        "Why? I thought you liked it. You said it made you remember when you were little.",
        "Yeah, that's the problem, I like it too much. Since you brought me some the first time, I've been wanting to drink more. So, I recklessly spent all of my credits on a shipment of blue milk.",
        "You did what?? This stuff is really expensive. How much did it cost you?",
        "Too much... ohh, don't make me think about it. Please accept this as a sign of gratitude and never mention blue milk again when speaking to me.",
        "Geez, I'm sorry. And thanks. I didn't know it would get you so addicted. If I'd known...",
        "It's ok, not your fault. Now please, stop talking about the drink.",
    };

    private List<bool> Cara_5_1_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<DialogImages> Cara_5_1_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ASHAMED,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ASHAMED,
        DialogImages.IMG_CARA_ASHAMED,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_5_2 = new List<string>()
    {
        "This is outrageous!",
        "Whoa, what happened? You seem agitated.",
        "Agitated? Agitated is nothing, I'm furious. They've fired me, you know. They really did it. Those vuvrian toydarian wretched...",
        "Fired? You? When? How? Why?",
        "It's not right, they can't do that. They can't fire me if we don't let them. You'll help me, right? We'll show them what we did to corrupt empire officers back in the rebellion army.",
        "Please, calm down and speak slower. You're not making any sense. I can't help you like this.",
        "Argh... fine. Ok, I'm alright now. The thing is, I'm not the Marshall anymore. They've taken my badge. ",
        "But I'm not going to stand still and do nothing while they sit on their comfortable golden thrones up in their floating privileged mansions toying with us like marionettes. I'm starting a new rebellion.",
    };

    private List<bool> Cara_5_2_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
    };

    private List<DialogImages> Cara_5_2_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
    };

    private List<string> Cara_5_3 = new List<string>()
    {
        "You are going to rebel against the former rebellion? This makes no sense. You fought for their cause.",
        "I fought for my home world. And they're no longer the Rebel Alliance, now they're just a bunch of corrupt politicians who are trying to take out our rights.",
        "I don't get it. What did you do to get fired? Did you tell them all this?",
        "No, not this. My shipment of blue milk was confiscated in a control point of the republic. I contacted one of my superiors and reclaimed my shipment, but she declined my petition. Then I got into an argument with her and she had me fired.",
        "Oh no, don't tell me you bought the milk from smugglers...",
        "You're missing the point here. I spent all of my money in that shipment, I am... was the Marshall, they should have turned it over to me.",
        "And what did you say to that superior?",
        "I just compared what they were doing with the way the empire treated underdeveloped planets that refused to submit to their authority. They put blockades so that they couldn't get any food or medicine from the outside. ",
        "They were doing the same thing to me! It's very unfair, right?",
        "Well, uh... maybe?",
        "After all I've done for you, you're not even on my side here? Fine, I'll find others who will join my cause, but don't expect any more help from me, you are alone in that Mandalore business of yours, and you'll have to find that jedi by yourself. ",
        "Just wait and see... I'll show them a new rebellion.",
        "No, but I just... Well, I hope it goes well for you...",
        // "No, but I just... Well, I hope it goes well for you. (To Grogu) Honestly, it's not that difficult to see why they fired her.",
    };

    private List<bool> Cara_5_3_bool = new List<bool>()
    {
		// Mando true, other false
        true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
        false,
        true,
        false,
        false,
        true,
    };

    private List<DialogImages> Cara_5_3_images = new List<DialogImages>()
    {
		// Mando true, other false
        DialogImages.IMG_MANDO_JOKING,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ASHAMED,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_SERIOUS,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_MANDO_ASHAMED,
        DialogImages.IMG_CARA_ANGRY2,
        DialogImages.IMG_CARA_ANGRY1,
        DialogImages.IMG_MANDO_ASHAMED,
    };

    public List<String> GetListOfDialog(uint index)
    {
        switch (index)
        {
            case 0:
                return Initial_Cutscene;
            case 1:
                return Bo_Katan_0_1;
            case 2:
                return Bo_Katan_0_2;
            case 3:
                return Bo_Katan_0_3;
            case 4:
                return Bo_Katan_1_1;
            case 5:
                return Bo_Katan_1_2;
            case 6:
                return Bo_Katan_1_3;
            case 7:
                return Bo_Katan_2_1;
            case 8:
                return Bo_Katan_2_2;
            case 9:
                return Bo_Katan_2_3;
            case 10:
                return Bo_Katan_3_1;
            case 11:
                return Bo_Katan_3_2;
            case 12:
                return Bo_Katan_3_3;
            case 13:
                return Bo_Katan_4_1;
            case 14:
                return Bo_Katan_4_2;
            case 15:
                return Bo_Katan_4_3;
            case 16:
                return Bo_Katan_5_1;
            case 17:
                return Bo_Katan_5_2;
            case 18:
                return Bo_Katan_5_3;
            case 19:
                return Greef_0_1;
            case 20:
                return Greef_0_2;
            case 21:
                return Greef_0_3;
            case 22:
                return Greef_1_1;
            case 23:
                return Greef_1_2;
            case 24:
                return Greef_1_3;
            case 25:
                return Greef_2_1;
            case 26:
                return Greef_2_2;
            case 27:
                return Greef_2_3;
            case 28:
                return Greef_3_1;
            case 29:
                return Greef_3_2;
            case 30:
                return Greef_3_3;
            case 31:
                return Greef_4_1;
            case 32:
                return Greef_4_2;
            case 33:
                return Greef_4_3;
            case 34:
                return Greef_5_1;
            case 35:
                return Greef_5_2;
            case 36:
                return Greef_5_3;
            case 37:
                return Ahsoka_0_1;
            case 38:
                return Ahsoka_0_2;
            case 39:
                return Ahsoka_0_3;
            case 40:
                return Ahsoka_1_1;
            case 41:
                return Ahsoka_1_2;
            case 42:
                return Ahsoka_1_3;
            case 43:
                return Ahsoka_2_1;
            case 44:
                return Ahsoka_2_2;
            case 45:
                return Ahsoka_2_3;
            case 46:
                return Ahsoka_3_1;
            case 47:
                return Ahsoka_3_2;
            case 48:
                return Ahsoka_3_3;
            case 49:
                return Ahsoka_4_1;
            case 50:
                return Ahsoka_4_2;
            case 51:
                return Ahsoka_4_3;
            case 52:
                return Ahsoka_5_1;
            case 53:
                return Ahsoka_5_2;
            case 54:
                return Ahsoka_5_3;
            case 55:
                return Cara_0_1;
            case 56:
                return Cara_0_2;
            case 57:
                return Cara_0_3;
            case 58:
                return Cara_1_1;
            case 59:
                return Cara_1_2;
            case 60:
                return Cara_1_3;
            case 61:
                return Cara_2_1;
            case 62:
                return Cara_2_2;
            case 63:
                return Cara_2_3;
            case 64:
                return Cara_3_1;
            case 65:
                return Cara_3_2;
            case 66:
                return Cara_3_3;
            case 67:
                return Cara_4_1;
            case 68:
                return Cara_4_2;
            case 69:
                return Cara_4_3;
            case 70:
                return Cara_5_1;
            case 71:
                return Cara_5_2;
            case 72:
                return Cara_5_3;
            case 73:
                return Grogu_0_1;
            case 74:
                return Grogu_0_2;
            case 75:
                return Grogu_1_1;
            case 76:
                return Grogu_1_2;
            case 77:
                return Grogu_2_1;
            case 78:
                return Grogu_2_2;
            case 79:
                return Grogu_3_1;
            case 80:
                return Grogu_3_2;
            case 81:
                return Grogu_4_1;
            case 82:
                return Grogu_4_2;
            case 83:
                return Grogu_5_1;
            case 84:
                return Grogu_5_2;
            case 85:
                return Final_Cutscene;
            case 86:
                return Post_Initial_Cutscene_Dialogue;
            case 87:
                return Hoth_Cutscene;
            case 88:
                return Ahsoka_PreBoss_Dialogue;
            case 89:
                return Cara_PreBoss_Dialogue;
            case 90:
                return Greef_PreBoss_Dialogue;
            case 91:
                return Bo_Katan_PreBoss_Dialogue;
            case 92:
                return Grogu_PreBoss_Dialogue;
            case 93:
                return Initial_Cutscene_Dialogue;
               
        }
        return Initial_Cutscene;

    }

    public List<bool> GetListOfOrder(uint index)
    {
        switch (index)
        {
            case 0:
                return Initial_Cutscene_bool;
            case 1:
                return Bo_Katan_0_1_bool;
            case 2:
                return Bo_Katan_0_2_bool;
            case 3:
                return Bo_Katan_0_3_bool;
            case 4:
                return Bo_Katan_1_1_bool;
            case 5:
                return Bo_Katan_1_2_bool;
            case 6:
                return Bo_Katan_1_3_bool;
            case 7:
                return Bo_Katan_2_1_bool;
            case 8:
                return Bo_Katan_2_2_bool;
            case 9:
                return Bo_Katan_2_3_bool;
            case 10:
                return Bo_Katan_3_1_bool;
            case 11:
                return Bo_Katan_3_2_bool;
            case 12:
                return Bo_Katan_3_3_bool;
            case 13:
                return Bo_Katan_4_1_bool;
            case 14:
                return Bo_Katan_4_2_bool;
            case 15:
                return Bo_Katan_4_3_bool;
            case 16:
                return Bo_Katan_5_1_bool;
            case 17:
                return Bo_Katan_5_2_bool;
            case 18:
                return Bo_Katan_5_3_bool;
            case 19:
                return Greef_0_1_bool;
            case 20:
                return Greef_0_2_bool;
            case 21:
                return Greef_0_3_bool;
            case 22:
                return Greef_1_1_bool;
            case 23:
                return Greef_1_2_bool;
            case 24:
                return Greef_1_3_bool;
            case 25:
                return Greef_2_1_bool;
            case 26:
                return Greef_2_2_bool;
            case 27:
                return Greef_2_3_bool;
            case 28:
                return Greef_3_1_bool;
            case 29:
                return Greef_3_2_bool;
            case 30:
                return Greef_3_3_bool;
            case 31:
                return Greef_4_1_bool;
            case 32:
                return Greef_4_2_bool;
            case 33:
                return Greef_4_3_bool;
            case 34:
                return Greef_5_1_bool;
            case 35:
                return Greef_5_2_bool;
            case 36:
                return Greef_5_3_bool;
            case 37:
                return Ahsoka_0_1_bool;
            case 38:
                return Ahsoka_0_2_bool;
            case 39:
                return Ahsoka_0_3_bool;
            case 40:
                return Ahsoka_1_1_bool;
            case 41:
                return Ahsoka_1_2_bool;
            case 42:
                return Ahsoka_1_3_bool;
            case 43:
                return Ahsoka_2_1_bool;
            case 44:
                return Ahsoka_2_2_bool;
            case 45:
                return Ahsoka_2_3_bool;
            case 46:
                return Ahsoka_3_1_bool;
            case 47:
                return Ahsoka_3_2_bool;
            case 48:
                return Ahsoka_3_3_bool;
            case 49:
                return Ahsoka_4_1_bool;
            case 50:
                return Ahsoka_4_2_bool;
            case 51:
                return Ahsoka_4_3_bool;
            case 52:
                return Ahsoka_5_1_bool;
            case 53:
                return Ahsoka_5_2_bool;
            case 54:
                return Ahsoka_5_3_bool;
            case 55:
                return Cara_0_1_bool;
            case 56:
                return Cara_0_2_bool;
            case 57:
                return Cara_0_3_bool;
            case 58:
                return Cara_1_1_bool;
            case 59:
                return Cara_1_2_bool;
            case 60:
                return Cara_1_3_bool;
            case 61:
                return Cara_2_1_bool;
            case 62:
                return Cara_2_2_bool;
            case 63:
                return Cara_2_3_bool;
            case 64:
                return Cara_3_1_bool;
            case 65:
                return Cara_3_2_bool;
            case 66:
                return Cara_3_3_bool;
            case 67:
                return Cara_4_1_bool;
            case 68:
                return Cara_4_2_bool;
            case 69:
                return Cara_4_3_bool;
            case 70:
                return Cara_5_1_bool;
            case 71:
                return Cara_5_2_bool;
            case 72:
                return Cara_5_3_bool;
            case 73:
                return Grogu_0_1_bool;
            case 74:
                return Grogu_0_2_bool;
            case 75:
                return Grogu_1_1_bool;
            case 76:
                return Grogu_1_2_bool;
            case 77:
                return Grogu_2_1_bool;
            case 78:
                return Grogu_2_2_bool;
            case 79:
                return Grogu_3_1_bool;
            case 80:
                return Grogu_3_2_bool;
            case 81:
                return Grogu_4_1_bool;
            case 82:
                return Grogu_4_2_bool;
            case 83:
                return Grogu_5_1_bool;
            case 84:
                return Grogu_5_2_bool;
            case 85:
                return Final_Cutscene_bool;
            case 86:
                return Post_Initial_Cutscene_Dialogue_bool;
            case 87:
                return Hoth_Cutscene_bool;
            case 88:
                return Ahsoka_PreBoss_Dialogue_bool;
            case 89:
                return Cara_PreBoss_Dialogue_bool;
            case 90:
                return Greef_PreBoss_Dialogue_bool;
            case 91:
                return Bo_Katan_PreBoss_Dialogue_bool;
            case 92:
                return Grogu_PreBoss_Dialogue_bool;
            case 93:
                return Initial_Cutscene_Dialogue_bool;
        }
        return Initial_Cutscene_bool;
    }

    public List<DialogImages> GetListOfImages(uint index)
    {
        switch (index)
        {
            case 0:
                return Initial_Cutscene_images;
            case 1:
                return Bo_Katan_0_1_images;
            case 2:
                return Bo_Katan_0_2_images;
            case 3:
                return Bo_Katan_0_3_images;
            case 4:
                return Bo_Katan_1_1_images;
            case 5:
                return Bo_Katan_1_2_images;
            case 6:
                return Bo_Katan_1_3_images;
            case 7:
                return Bo_Katan_2_1_images;
            case 8:
                return Bo_Katan_2_2_images;
            case 9:
                return Bo_Katan_2_3_images;
            case 10:
                return Bo_Katan_3_1_images;
            case 11:
                return Bo_Katan_3_2_images;
            case 12:
                return Bo_Katan_3_3_images;
            case 13:
                return Bo_Katan_4_1_images;
            case 14:
                return Bo_Katan_4_2_images;
            case 15:
                return Bo_Katan_4_3_images;
            case 16:
                return Bo_Katan_5_1_images;
            case 17:
                return Bo_Katan_5_2_images;
            case 18:
                return Bo_Katan_5_3_images;
            case 19:
                return Greef_0_1_images;
            case 20:
                return Greef_0_2_images;
            case 21:
                return Greef_0_3_images;
            case 22:
                return Greef_1_1_images;
            case 23:
                return Greef_1_2_images;
            case 24:
                return Greef_1_3_images;
            case 25:
                return Greef_2_1_images;
            case 26:
                return Greef_2_2_images;
            case 27:
                return Greef_2_3_images;
            case 28:
                return Greef_3_1_images;
            case 29:
                return Greef_3_2_images;
            case 30:
                return Greef_3_3_images;
            case 31:
                return Greef_4_1_images;
            case 32:
                return Greef_4_2_images;
            case 33:
                return Greef_4_3_images;
            case 34:
                return Greef_5_1_images;
            case 35:
                return Greef_5_2_images;
            case 36:
                return Greef_5_3_images;
            case 37:
                return Ahsoka_0_1_images;
            case 38:
                return Ahsoka_0_2_images;
            case 39:
                return Ahsoka_0_3_images;
            case 40:
                return Ahsoka_1_1_images;
            case 41:
                return Ahsoka_1_2_images;
            case 42:
                return Ahsoka_1_3_images;
            case 43:
                return Ahsoka_2_1_images;
            case 44:
                return Ahsoka_2_2_images;
            case 45:
                return Ahsoka_2_3_images;
            case 46:
                return Ahsoka_3_1_images;
            case 47:
                return Ahsoka_3_2_images;
            case 48:
                return Ahsoka_3_3_images;
            case 49:
                return Ahsoka_4_1_images;
            case 50:
                return Ahsoka_4_2_images;
            case 51:
                return Ahsoka_4_3_images;
            case 52:
                return Ahsoka_5_1_images;
            case 53:
                return Ahsoka_5_2_images;
            case 54:
                return Ahsoka_5_3_images;
            case 55:
                return Cara_0_1_images;
            case 56:
                return Cara_0_2_images;
            case 57:
                return Cara_0_3_images;
            case 58:
                return Cara_1_1_images;
            case 59:
                return Cara_1_2_images;
            case 60:
                return Cara_1_3_images;
            case 61:
                return Cara_2_1_images;
            case 62:
                return Cara_2_2_images;
            case 63:
                return Cara_2_3_images;
            case 64:
                return Cara_3_1_images;
            case 65:
                return Cara_3_2_images;
            case 66:
                return Cara_3_3_images;
            case 67:
                return Cara_4_1_images;
            case 68:
                return Cara_4_2_images;
            case 69:
                return Cara_4_3_images;
            case 70:
                return Cara_5_1_images;
            case 71:
                return Cara_5_2_images;
            case 72:
                return Cara_5_3_images;
            case 73:
                return Grogu_0_1_images;
            case 74:
                return Grogu_0_2_images;
            case 75:
                return Grogu_1_1_images;
            case 76:
                return Grogu_1_2_images;
            case 77:
                return Grogu_2_1_images;
            case 78:
                return Grogu_2_2_images;
            case 79:
                return Grogu_3_1_images;
            case 80:
                return Grogu_3_2_images;
            case 81:
                return Grogu_4_1_images;
            case 82:
                return Grogu_4_2_images;
            case 83:
                return Grogu_5_1_images;
            case 84:
                return Grogu_5_2_images;
            case 85:
                return Final_Cutscene_images;
            case 86:
                return Post_Initial_Cutscene_Dialogue_images;
            case 87:
                return Hoth_Cutscene_images;
            case 88:
                return Ahsoka_PreBoss_Dialogue_images;
            case 89:
                return Cara_PreBoss_Dialogue_images;
            case 90:
                return Greef_PreBoss_Dialogue_images;
            case 91:
                return Bo_Katan_PreBoss_Dialogue_images;
            case 92:
                return Grogu_PreBoss_Dialogue_images;
            case 93:
                return Initial_Cutscene_Dialogue_images;
        }
        return Initial_Cutscene_images;
    }

    public String GetDialogName(uint index)
    {
        switch (index)
        {
            case 0:
                return "Grogu";
            case 1:
                return "Bo-Katan";
            case 2:
                return "Bo-Katan";
            case 3:
                return "Bo-Katan";
            case 4:
                return "Bo-Katan";
            case 5:
                return "Bo-Katan";
            case 6:
                return "Bo-Katan";
            case 7:
                return "Bo-Katan";
            case 8:
                return "Bo-Katan";
            case 9:
                return "Bo-Katan";
            case 10:
                return "Bo-Katan";
            case 11:
                return "Bo-Katan";
            case 12:
                return "Bo-Katan";
            case 13:
                return "Bo-Katan";
            case 14:
                return "Bo-Katan";
            case 15:
                return "Bo-Katan";
            case 16:
                return "Bo-Katan";
            case 17:
                return "Bo-Katan";
            case 18:
                return "Bo-Katan";
            case 19:
                return "Greef";
            case 20:
                return "Greef";
            case 21:
                return "Greef";
            case 22:
                return "Greef";
            case 23:
                return "Greef";
            case 24:
                return "Greef";
            case 25:
                return "Greef";
            case 26:
                return "Greef";
            case 27:
                return "Greef";
            case 28:
                return "Greef";
            case 29:
                return "Greef";
            case 30:
                return "Greef";
            case 31:
                return "Greef";
            case 32:
                return "Greef";
            case 33:
                return "Greef";
            case 34:
                return "Greef";
            case 35:
                return "Greef";
            case 36:
                return "Greef";
            case 37:
                return "Ahsoka";
            case 38:
                return "Ahsoka";
            case 39:
                return "Ahsoka";
            case 40:
                return "Ahsoka";
            case 41:
                return "Ahsoka";
            case 42:
                return "Ahsoka";
            case 43:
                return "Ahsoka";
            case 44:
                return "Ahsoka";
            case 45:
                return "Ahsoka";
            case 46:
                return "Ahsoka";
            case 47:
                return "Ahsoka";
            case 48:
                return "Ahsoka";
            case 49:
                return "Ahsoka";
            case 50:
                return "Ahsoka";
            case 51:
                return "Ahsoka";
            case 52:
                return "Ahsoka";
            case 53:
                return "Ahsoka";
            case 54:
                return "Ahsoka";
            case 55:
                return "Cara";
            case 56:
                return "Cara";
            case 57:
                return "Cara";
            case 58:
                return "Cara";
            case 59:
                return "Cara";
            case 60:
                return "Cara";
            case 61:
                return "Cara";
            case 62:
                return "Cara";
            case 63:
                return "Cara";
            case 64:
                return "Cara";
            case 65:
                return "Cara";
            case 66:
                return "Cara";
            case 67:
                return "Cara";
            case 68:
                return "Cara";
            case 69:
                return "Cara";
            case 70:
                return "Cara";
            case 71:
                return "Cara";
            case 72:
                return "Cara";
            case 73:
                return "Grogu";
            case 74:
                return "Grogu";
            case 75:
                return "Grogu";
            case 76:
                return "Grogu";
            case 77:
                return "Grogu";
            case 78:
                return "Grogu";
            case 79:
                return "Grogu";
            case 80:
                return "Grogu";
            case 81:
                return "Grogu";
            case 82:
                return "Grogu";
            case 83:
                return "Grogu";
            case 84:
                return "Grogu";
            case 85:
                return "Bo-Katan";
            case 86:
                return "Greef";
            case 87:
                return "Ahsoka";
            case 88:
                return "Ahsoka";
            case 89:
                return "Cara";
            case 90:
                return "Greef";
            case 91:
                return "Bo-Katan";
            case 92:
                return "Grogu";
        }
        return " ";

    }

}