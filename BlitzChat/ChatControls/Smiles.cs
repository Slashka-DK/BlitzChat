﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
namespace BlitzChat
{
    public class Smiles
    {
        MainWindow main;
        public Hashtable emotions;
        public Smiles(MainWindow m) {
            main = m;
            CreateEmotions();
        }

        public void CreateEmotions()
        {
            string path = "pack://application:,,,/BlitzChat;component/smiles/";
            emotions = new Hashtable();
            emotions.Add(">(", path+"angry.png");
            emotions.Add(":D", path+"big_grin.png");
            emotions.Add(":z", path+"bored.png");
            emotions.Add("o_O", path+"confused.png");
            emotions.Add(":B)", path+"cool.png");
            emotions.Add("<3", path+"heart.png");
            emotions.Add("R)", path+"pirate.png");
            emotions.Add(":(", path+"sad.png");
            emotions.Add(":)", path+"smile.png");
            emotions.Add(":P", path+"sticking_tongue_out.png");
            emotions.Add(":o", path+"surprised.png");
            emotions.Add(":\\", path+"undecided.png");
            emotions.Add(";p", path+"wink.png");
            emotions.Add(";)", path+"winking.png");
            emotions.Add(":7", path+"smoking.png");
            emotions.Add(":|", path+"sleeping.png");
            emotions.Add("#/", path+"ninja.png");
            emotions.Add(":s", path+"mouth_shut.png");
            emotions.Add(":>", path+"love_it.png");
            emotions.Add("<]", path+"dunce.png");
            emotions.Add("4Head", path+"4head.png");
            emotions.Add("ArsonNoSexy", path+"arsonnosexy.png");
            emotions.Add("AsianGlow", path+"asianglow.png");
            emotions.Add("AtGL", path+"atgl.png");
            emotions.Add("AtIvy", path+"ativy.png");
            emotions.Add("AtWW", path+"atww.png");
            emotions.Add("BCWarrior", path+"bcwarrior.png");
            emotions.Add("BORT", path+"bort.png");
            emotions.Add("BatChest", path+"batchest.png");
            emotions.Add("BibleThump", path+"biblethump.png");
            emotions.Add("BionicBunion", path+"bionicbunion.png");
            emotions.Add("BlargNaut", path+"blargnaut.png");
            emotions.Add("BloodTrail", path+"bloodtrail.png");
            emotions.Add("BrainSlug", path+"brainslug.png");
            emotions.Add("BrokeBack", path+"brokeback.png");
            emotions.Add("CougarHunt", path+"cougarhunt.png");
            emotions.Add("DAESuppy", path+"daesuppy.png");
            emotions.Add("DBstyle", path+"dbstyle.png");
            emotions.Add("DansGame", path+"dansgame.png");
            emotions.Add("DatSheffy", path+"datsheffy.png");
            emotions.Add("DogFace", path+"dogface.png");
            emotions.Add("EagleEye", path+"eagleeye.png");
            emotions.Add("EleGiggle", path+"elegiggle.png");
            emotions.Add("EvilFetus", path+"evilfetus.png");
            emotions.Add("FPSMarksman", path+"fpsmarksman.png");
            emotions.Add("FUNgineer", path+"fungineer.png");
            emotions.Add("FailFish", path+"failfish.png");
            emotions.Add("FrankerZ", path+"frankerz.png");
            emotions.Add("FreakinStinkin", path+"freakinstinkin.png");
            emotions.Add("FuzzyOtterOO", path+"fuzzyotteroo.png");
            emotions.Add("GasJoker", path+"gasjoker.png");
            emotions.Add("GingerPower", path+"gingerpower.png");
            emotions.Add("GrammarKing", path+"grammarking.png");
            emotions.Add("HassanChop", path+"hassanchop.png");
            emotions.Add("HotPokket", path+"hotpokket.png");
            emotions.Add("ItsBoshyTime", path+"itsboshytime.png");
            emotions.Add("JKanStyle", path+"jkanstyle.png");
            emotions.Add("Jebaited", path+"jebaited.png");
            emotions.Add("JonCarnage", path+"joncarnage.png");
            emotions.Add("KAPOW", path+"kapow.png");
            emotions.Add("KZassault", path+"kzassault.png");
            emotions.Add("KZcover", path+"kzcover.png");
            emotions.Add("KZguerilla", path+"kzguerilla.png");
            emotions.Add("KZhelghast", path+"kzhelghast.png");
            emotions.Add("KZowl", path+"kzowl.png");
            emotions.Add("KZskull", path+"kzskull.png");
            emotions.Add("Kappa", path+"kappa.png");
            emotions.Add("KappaHD", path+"kappahd.png");
            emotions.Add("Keepo", path+"keepo.png");
            emotions.Add("KevinTurtle", path+"kevinturtle.png");
            emotions.Add("Kippa", path+"kippa.png");
            emotions.Add("Kreygasm", path+"kreygasm.png");
            emotions.Add("MVGame", path+"mvgame.png");
            emotions.Add("MechaSupes", path+"mechasupes.png");
            emotions.Add("MiniK", path+"minik.png");
            emotions.Add("MrDestructoid", path+"mrdestructoid.png");
            emotions.Add("NightBat", path+"nightbat.png");
            emotions.Add("NinjaTroll", path+"ninjatroll.png");
            emotions.Add("NoNoSpot", path+"nonospot.png");
            emotions.Add("OMGScoots", path+"omgscoots.png");
            emotions.Add("OneHand", path+"onehand.png");
            emotions.Add("OpieOP", path+"opieop.png");
            emotions.Add("OptimizePrime", path+"optimizeprime.png");
            emotions.Add("PJHarley", path+"pjharley.png");
            emotions.Add("PJSalt", path+"pjsalt.png");
            emotions.Add("PMSTwin", path+"pmstwin.png");
            emotions.Add("PanicVis", path+"panicvis.png");
            emotions.Add("PazPazowitz", path+"pazpazowitz.png");
            emotions.Add("PeoplesChamp", path+"peopleschamp.png");
            emotions.Add("PicoMause", path+"picomause.png");
            emotions.Add("PipeHype", path+"pipehype.png");
            emotions.Add("PogChamp", path+"pogchamp.png");
            emotions.Add("Poooound", path+"poooound.png");
            emotions.Add("PunchTrees", path+"punchtrees.png");
            emotions.Add("RalpherZ", path+"ralpherz.png");
            emotions.Add("RedCoat", path+"redcoat.png");
            emotions.Add("ResidentSleeper", path+"residentsleeper.png");
            emotions.Add("RitzMitz", path+"ritzmitz.png");
            emotions.Add("RuleFive", path+"rulefive.png");
            emotions.Add("SMOrc", path+"smorc.png");
            emotions.Add("SMSkull", path+"smskull.png");
            emotions.Add("SSSsss", path+"ssssss.png");
            emotions.Add("ShazBotstix", path+"shazbotstix.png");
            emotions.Add("Shazam", path+"shazam.png");
            emotions.Add("SoBayed", path+"sobayed.png");
            emotions.Add("SoonerLater", path+"soonerlater.png");
            emotions.Add("StoneLightning", path+"stonelightning.png");
            emotions.Add("StrawBeary", path+"strawbeary.png");
            emotions.Add("SuperVinlin", path+"supervinlin.png");
            emotions.Add("SwiftRage", path+"swiftrage.png");
            emotions.Add("TF2John", path+"tf2john.png");
            emotions.Add("TehFunrun", path+"tehfunrun.png");
            emotions.Add("TheRinger", path+"theringer.png");
            emotions.Add("TheTarFu", path+"thetarfu.png");
            emotions.Add("TheThing", path+"thething.png");
            emotions.Add("ThunBeast", path+"thunbeast.png");
            emotions.Add("TinyFace", path+"tinyface.png");
            emotions.Add("TooSpicy", path+"toospicy.png");
            emotions.Add("TriHard", path+"trihard.png");
            emotions.Add("UleetBackup", path+"uleetbackup.png");
            emotions.Add("UnSane", path+"unsane.png");
            emotions.Add("UncleNox", path+"unclenox.png");
            emotions.Add("Volcania", path+"volcania.png");
            emotions.Add("WTRuck", path+"wtruck.png");
            emotions.Add("WholeWheat", path+"wholewheat.png");
            emotions.Add("WinWaker", path+"winwaker.png");
            emotions.Add("YouWHY", path+"youwhy.png");
            emotions.Add("aneleanele", path+"aneleanele.png");
            emotions.Add("noScope420", path+"noscope420.png");
            emotions.Add("shazamicon", path+"shazamicon.png");
            emotions.Add(":happy:", path + "a.png");
            emotions.Add(":aws:", path + "awesome.png");
            emotions.Add(":nc:", path + "nocomments.png");
            emotions.Add(":manul:", path + "manul.png");
            emotions.Add(":crazy:", path + "crazy.png");
            emotions.Add(":cry:", path + "cry.png");
            emotions.Add(":glory:", path + "glory.png");
            emotions.Add(":kawai:", path + "kawai.png");
            emotions.Add(":mee:", path + "mee.png");
            emotions.Add(":omg:", path + "omg.png");
            emotions.Add(":whut:", path + "mhu.png");
            emotions.Add(":sad:", path + "sadsc.png");
            emotions.Add(":spk:", path + "slowpoke.png");
            emotions.Add(":hmhm:", path + "2.png");
            emotions.Add(":mad:", path + "mad.png");
            emotions.Add(":angry:", path + "aangry.png");
            emotions.Add(":xd:", path + "ii.png");
            emotions.Add(":huh:", path + "huh.png");
            emotions.Add(":tears:", path + "happycry.png");
            emotions.Add(":notch:", path + "notch.png");
            emotions.Add(":vaga:", path + "vaganych.png");
            emotions.Add(":ra:", path + "ra.png");
            emotions.Add(":fp:", path + "facepalm.png");
            emotions.Add(":neo:", path + "smith.png");
            emotions.Add(":peka:", path + "mini-happy.png");
            emotions.Add(":trf:", path + "trollface.png");
            emotions.Add(":fu:", path + "fuuuu.png");
            emotions.Add(":why:", path + "why.png");
            emotions.Add(":yao:", path + "yao.png");
            emotions.Add(":fyeah:", path + "fyeah.png");
            emotions.Add(":lucky:", path + "lol.png");
            emotions.Add(":okay:", path + "okay.png");
            emotions.Add(":alone:", path + "alone.png");
            emotions.Add(":joyful:", path + "ewbte.png");
            emotions.Add(":wtf:", path + "wtf.png");
            emotions.Add(":danu:", path + "daladno.png");
            emotions.Add(":gusta:", path + "megusta.png");
            emotions.Add(":bm:", path + "bm.png");
            emotions.Add(":lol:", path+"loool.png");
            emotions.Add(":notbad:", path+"notbad.png");
            emotions.Add(":rly:", path+"really.png");
            emotions.Add(":ban:", path+"banan.png");
            emotions.Add(":cap:", path+"cap.png");
            emotions.Add(":br:", path+"br.png");
            emotions.Add(":fpl:", path+"leefacepalm.png");
            emotions.Add(":ht:", path+"heartsc.png");
            emotions.Add(":adolf:", path+"adolf.png");
            emotions.Add(":bratok:", path+"bratok.png");
            emotions.Add(":strelok:", path+"strelok.png");
            emotions.Add(":white-ra:", path+"white-ra.png");
            emotions.Add(":dimaga:", path+"dimaga.png");
            emotions.Add(":bruce:", path+"bruce.png");
            emotions.Add(":jae:", path+"jaedong.png");
            emotions.Add(":flash:", path+"flash1.png");
            emotions.Add(":bisu:", path+"bisu.png");
            emotions.Add(":jangbi:", path+"jangbi.png");
            emotions.Add(":idra:", path+"idra.png");
            emotions.Add(":vdv:", path+"vitya.png");
            emotions.Add(":imba:", path+"djigurda.png");
            emotions.Add(":chuck:", path+"chan.png");
            emotions.Add(":tgirl:", path+"brucelove.png");
            emotions.Add(":top1sng:", path+"happy.png");
            emotions.Add(":slavik:", path+"slavik.png");
            emotions.Add(":olsilove:", path+"olsilove.png");
            emotions.Add(":kas:", path+"kas.png");
            emotions.Add(":pool:", path+"pool.png");
            emotions.Add(":ej:", path+"ejik.png");
            emotions.Add(":mario:", path+"mario.png");
            emotions.Add(":tort:", path+"tort.png");
            emotions.Add(":arni:", path+"terminator.png");
            emotions.Add(":crab:", path+"crab.png");
            emotions.Add(":hero:", path+"heroes3.png");
            emotions.Add(":mc:", path + "ssssss.png");
            emotions.Add(":osu:", path+"osu.png");
            emotions.Add(":q3:", path+"q3.png");
            emotions.Add(":tigra:", path+"tigrica.png");
            emotions.Add(":volck:", path+"voOlchik1.png");
            emotions.Add(":hpeka:", path+"harupeka.png");
            emotions.Add(":slow:", path+"spok.png");
            emotions.Add(":alex:", path+"alfi.png");
            emotions.Add(":panda:", path+"panda.png");
            emotions.Add(":sun:", path+"sunl.png");
            emotions.Add(":cou:", path+"cougar.png");
            emotions.Add(":wb:", path+"wormban.png");
            emotions.Add(":dobro:", path+"dobre.png");
            emotions.Add(":theweedle:", path+"weedle.png");
            emotions.Add(":apc:", path+"apochai.png");
            emotions.Add(":globus:", path+"globus.png");
            emotions.Add(":cow:", path+"cow.png");
            emotions.Add(":nook:", path+"no-okay.png");
            emotions.Add(":noj:", path+"knife.png");
            emotions.Add(":fpd:", path+"fp.png");
            emotions.Add(":hg:", path+"hg.png");
            emotions.Add(":yoko:", path+"yoko.png");
            emotions.Add(":miku:", path+"miku.png");
            emotions.Add(":winry:", path+"winry.png");
            emotions.Add(":asuka:", path+"asuka.png");
            emotions.Add(":konata:", path+"konata.png");
            emotions.Add(":reimu:", path+"reimu.png");
            emotions.Add(":sex:", path+"sex.png");
            emotions.Add(":mimo:", path+"mimo.png");
            emotions.Add(":fire:", path+"fire.png");
            emotions.Add(":mandarin:", path+"mandarin.png");
            emotions.Add(":grafon:", path+"grafon.png");
            emotions.Add(":epeka:", path+"epeka.png");
            emotions.Add(":opeka:", path+"opeka.png");
            emotions.Add(":ocry:", path+"ocry.png");
            emotions.Add(":neponi:", path+"neponi.png");
            emotions.Add(":moon:", path+"moon.png");
            emotions.Add(":ghost:", path+"gay.png");
            emotions.Add(":omsk:", path+"omsk.png");
            emotions.Add(":grumpy:", path+"grumpy.png");
            emotions.Add(":bobr:", path+"bobr.png");
            emotions.Add(":yeah:" , path+"yeah.png");
            emotions.Add(":probe:" , path+"probe.png");
            emotions.Add(":ling:" , path+"ling.png");
            emotions.Add(":marine:" , path+"marine.png");
            emotions.Add(":mvp:" , path+"mvp.png");
            emotions.Add(":bin:" , path+"bin.png");
            emotions.Add(":kim:" , path+"kim.png");
            emotions.Add(":kot:" , path+"kot.png");
            emotions.Add(":sm1:" , path+"sm1.png");
            emotions.Add(":warn:" , path+"warn.png");
            emotions.Add(":trash:" , path+"trash.png");
            emotions.Add(":blue:" , path+"blue.png");
            emotions.Add(":gold:" , path+"gold.png");
            emotions.Add(":grey:" , path+"grey.png");
            emotions.Add(":purp:" , path+"purp.png");
            emotions.Add(":red:" , path+"red.png");
            emotions.Add(":daaa:" , path+"daaa.png");
            emotions.Add(":gg:" , path+"gg.png");
            emotions.Add(":op:" , path+"op.png");
            emotions.Add(":666:" , path+"666.png");
            emotions.Add(":bear:" , path+"bear.png");
            emotions.Add(":drone:" , path+"drone.png");
            emotions.Add(":lgun:" , path+"lgun.png");
            emotions.Add(":lknife:" , path+"lknife.png");
            emotions.Add(":mule:" , path+"mule.png");
            emotions.Add(":no:" , path+"no.png");
            emotions.Add(":ploho:" , path+"ploho.png");
            emotions.Add(":pushka:" , path+"pushka.png");
            emotions.Add(":rgun:" , path+"rgun.png");
            emotions.Add(":rknife:" , path+"rknife.png");
            emotions.Add(":support:" , path+"support.png");
            emotions.Add(":plasma:" , path+"plasma.png");
            emotions.Add(":ff:" , path+"ff.png");
            emotions.Add(":rage:" , path+"rage.png");
            emotions.Add(":putin:" , path+"putin.png");
            emotions.Add(":pled:" , path+"pled.png");
            emotions.Add(":zmbeka:" , path+"zmbeka.png");
            emotions.Add(":wow:" , path+"wow.png");
            emotions.Add(":usa:" , path+"usa.png");
            emotions.Add(":ukr:" , path+"ukr.png");
            emotions.Add(":stalk:" , path+"stalk.png");
            emotions.Add(":sir:" , path+"sir.png");
            emotions.Add(":scv:" , path +"scv.png");
            emotions.Add(":rus:" , path +"rus.png");
            emotions.Add(":roach:" , path +"roach.png");
            emotions.Add(":pvrt:" , path +"pvrt.png");
            emotions.Add(":orcl:" , path +"orcl.png");
            emotions.Add(":norma:" , path +"norma.png");
            emotions.Add(":money:" , path +"money.png");
            emotions.Add(":mine:" , path +"mine.png");
            emotions.Add(":kor:" , path +"kor.png");
            emotions.Add(":kid3:" , path +"kid3.png");
            emotions.Add(":kid2:" , path +"kid2.png");
            emotions.Add(":guf:" , path +"guf.png");
            emotions.Add(":gglord:" , path +"gglord.png");
            emotions.Add(":genius:" , path +"genius.png");
            emotions.Add(":drama:" , path +"drama.png");
            emotions.Add(":db:" , path +"db.png");
            emotions.Add(":boyan:" , path +"boyan.png");
            emotions.Add(":gml:" , path +"gml.png");
            emotions.Add(":five:" , path +"five.png");
            emotions.Add(":denrus:" , path +"denrus.png");
            emotions.Add(":ronaldo:" , path +"ronaldo.png");
            emotions.Add(":zayka:" , path +"zayka.png");
            emotions.Add(":volnov:" , path +"volnov.png");
            emotions.Add(":ziga:" , path +"ziga.png");
            emotions.Add(":oscar:" , path +"oscar.png");
            emotions.Add(":kingalf:" , path +"alfi-crown.png");
            emotions.Add(":mofgod:" , path +"mother-of-the-god.png");
            emotions.Add(":prime:" , path +"fredy18.png");
            emotions.Add(":pekaking:" , path +"pekaking.png");
            emotions.Add(":ilied:" , path +"ilied.png");
            emotions.Add(":coupeka:" , path +"coupeka.png");
            emotions.Add(":gogo:" , path +"gogo.png");
            emotions.Add(":pekling:" , path +"pekling.png");
            emotions.Add(":zeal:" , path +"zeal.png");
            emotions.Add(":infe:" , path +"infe.png");
            emotions.Add(":cheese:" , path +"cheese.png");
            emotions.Add(":dark:" , path +"dark.png");
            emotions.Add(":reaper:" , path +"reaper.png");
            emotions.Add(":maro:" , path +"maro.png");
            emotions.Add(":butth:" , path +"butth.png");
            emotions.Add(":pri21:" , path +"pri21.png");
            emotions.Add(":daya:" , path +"daya.png");
            emotions.Add(":ggwp:" , path +"ggwp.png");
            emotions.Add(":imbaimba:" , path +"imbaimba.png");
            emotions.Add(":kid:" , path +"kid.png");
            emotions.Add(":num1:" , path +"num1.png");
            emotions.Add(":poker:" , path +"poker.png");
            emotions.Add(":povar:" , path +"povar.png");
            emotions.Add(":vuvu:" , path +"vuvu.png");
            emotions.Add(":angryling:" , path +"angryling.png");
            emotions.Add(":casterling:" , path +"casterling.png");
            emotions.Add(":kawailing:" , path +"kawailing.png");
            emotions.Add(":nelson:" , path +"nelson.png");
            emotions.Add(":richpeka:" , path +"richpeka.png");
            emotions.Add(":uprls:" , path +"uprls.png");
            emotions.Add(":vsem:" , path +"vsem.png");
            emotions.Add(":wat:" , path +"wat.png");
            emotions.Add(":yopeka:" , path +"yopeka.png");
            emotions.Add(":vesir:" , path +"vesir.png");
            emotions.Add(":vat:" , path +"vat.png");
            emotions.Add(":ukrpeka:" , path +"ukrpeka.png");
            emotions.Add(":rnbw:" , path +"rnbw.png");
            emotions.Add(":rip:" , path +"rip.png");
            emotions.Add(":izi:" , path +"izi.png");
            emotions.Add(":hmpeka:" , path +"hmpeka.png");
            emotions.Add(":ehai:" , path +"ehai.png");
        }

        public void checkEmotions(ref Paragraph par, double scalesize)
        {
            TextRange texR = new TextRange(par.ContentStart, par.ContentEnd);
            foreach (DictionaryEntry emote in emotions)
            {
                if (texR.Text.Contains(emote.Key.ToString())) {
                    for (int j=3;j<par.Inlines.Count;j++)
                    {
                        texR = new TextRange(par.Inlines.ElementAt(j).ContentStart, par.Inlines.ElementAt(j).ContentEnd);
                        string em = emote.Key.ToString();
                        em = em.Replace("(", "\\(");
                        em = em.Replace(")", "\\)");
                        if (texR.Text.Contains(emote.Key.ToString()) && Regex.IsMatch(texR.Text, String.Format(@"(^|\s){0}(\s|$)", em))) //Smiley in Text
                        {
                            TextPointer tp = par.Inlines.ElementAt(j).ContentStart;
                            while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(emote.Key.ToString()))
                                tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                            TextPointer endPoint = tp;
                            for (int i = 0; i < emote.Key.ToString().Length; i++)
                                endPoint = endPoint.GetNextInsertionPosition(LogicalDirection.Forward);
                            texR = new TextRange(tp, endPoint);
                            texR.Text = "";
                            Image img = new Image();
                            BitmapImage bitmapImage = new BitmapImage(new Uri(@emote.Value.ToString()));
                            //System.Drawing.Bitmap bitm = (System.Drawing.Bitmap)emotions[emote.Key];
                            double width = bitmapImage.Width; 
                            double height = bitmapImage.Height;
                            //width = (Math.Max(width, par.FontSize) - Math.Min(width, par.FontSize)) + Math.Min(width, par.FontSize);
                            //height = (Math.Max(height, par.FontSize) - Math.Min(height, par.FontSize)) + Math.Min(height, par.FontSize);
                            //width += width - par.FontSize;
                            //height += height - par.FontSize;
                            //System.Drawing.Color c;
                            //System.Drawing.Color replaceColor = System.Drawing.Color.FromArgb(255, 0, 0, 128);
                            //for (int w = 0; w < width; w++)
                            //    for (int h = 0; h < height; h++)
                            //    {
                            //        c = bitm.GetPixel(w, h);
                            //        bitm.SetPixel(w, h, ((((short)(c.A)) & 0x00FF) <= 0) ? replaceColor : c); //replace 0 here with some higher tolerance if needed
                            //    }
                            //bitm.MakeTransparent(System.Drawing.Color.FromArgb(255, 0, 0, 128));
                            //using (MemoryStream memory = new MemoryStream())
                            //{
                            //    bitm.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                            //    memory.Position = 0;
                            //    bitmapImage = new BitmapImage();
                            //    bitmapImage.BeginInit();
                            //    bitmapImage.StreamSource = memory;
                            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            //    bitmapImage.EndInit();
                            //}
                            img.Source = bitmapImage;
                            img.Stretch = Stretch.Fill;
                            img.Width = width+scalesize;
                            img.Height = height + scalesize;
                            new InlineUIContainer(img, tp);
                            //rtb.Focus();
                            break;
                        }
                    }
                }
            }
         
        }
    }
}
