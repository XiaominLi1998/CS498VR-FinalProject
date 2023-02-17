using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGreVocabData : MonoBehaviour
{
    public static List<Vocab> vocabs = new List<Vocab>();
    // Start is called before the first frame update
    void Start()
    {
       loadfile("GRE_vocab");
       loadfile("SAT_vocab");
       loadfile("TOEFL_vocab");
        
        //uncomment these following lines to test if vocabs are correctly imported
        // foreach (Vocab v in vocabs)
        // {
        //     Debug.Log(v.vocab);
        // }
    }
    public void loadfile(string filename) 
    {
        TextAsset vocab_= Resources.Load<TextAsset>(filename);
        string[] data = vocab_.text.Split(new char[] {'\n'});
        // skip the first line and the last line
        for (int i = 1; i < data.Length - 1; i++)
        {
            string[] row = data[i].Split(new char[] {','});
            Vocab v = new Vocab();
            int.TryParse(row[0],out v.id);
            v.vocab = row[1];
            v.meaning = row[2];
            
            vocabs.Add(v);

        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
