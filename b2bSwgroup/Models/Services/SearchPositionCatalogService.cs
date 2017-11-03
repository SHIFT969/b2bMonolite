using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util.Cache;
using Lucene.Net.Search.Spans;
using Lucene.Net.Search.Payloads;
using Lucene.Net.Search.Function;
using Lucene.Net.Store;
using System.Xml;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using b2bSwgroup.Models.ModelsForView;


namespace b2bSwgroup.Models.Services
{
    public class SearchPositionCatalogService
    {
        private ApplicationContext db = new ApplicationContext();
        //public async Task<IEnumerable<PositionCatalogView>> SearchAsync(string keywords, int limit)
        //{
        //    await BuildIndex();
        //    using (var directory = GetDirectory())
        //    using (var searcher = new IndexSearcher(directory))
        //    {
        //        var query = GetQuery(keywords);
        //        var docs = searcher.Search(query, limit);
        //        //count = docs.TotalHits;

        //        var products = new List<PositionCatalogView>();
        //        foreach (var scoreDoc in docs.ScoreDocs)
        //        {
        //            var doc = searcher.Doc(scoreDoc.Doc);
        //            var product = new PositionCatalogView
        //            {
        //                Id = int.Parse(doc.Get("id")),
        //                Articul = doc.Get("articul"),
        //                Name = doc.Get("name"),
        //                Category = doc.Get("category"),
        //                Distributor = doc.Get("distributor"),
        //                PartNumber = doc.Get("partnumber"),
        //                Price = doc.Get("price")
        //            };
        //            products.Add(product);
        //        }
        //        return products;
        //    }
        //}
        Query GetQuery(string keywords)
        {
            using (var analyzer = GetAnalyzer())
            {
                var query = new BooleanQuery();

                String[] keys = keywords.Split(new char[] { ' ', '\n', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                if (keywords == "")
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "name", analyzer);
                    parser.PhraseSlop = 50;
                    parser.FuzzyMinSim = 0.5f;

                    query.Add(parser.Parse("aple aspire acer"), Occur.SHOULD);

                }
                else
                {
                }

                return query;
            }
        }
        private async Task BuildIndex()
        {
            List<PositionCatalog> positions = new List<PositionCatalog>();

            positions = await db.Positionscatalog.Include(c => c.Category).Include(c => c.Currency).Include(d => d.Distributor).ToListAsync();

            using (var directory = GetDirectory())
            using (var analyzer = GetAnalyzer())
            using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.DeleteAll();
                foreach (var product in positions)
                {
                    var document = MapProduct(product);
                    writer.AddDocument(document);
                }
            }
        }
        Lucene.Net.Store.Directory GetDirectory()
        {
            //var dir = Directory.CreateDirectory(,)
            return new SimpleFSDirectory(new DirectoryInfo(HttpContext.Current.Server.MapPath("DataIndex")));
        }
        Analyzer GetAnalyzer()
        {
            return new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        }
        Document MapProduct(PositionCatalog position)
        {
            var document = new Document();
            document.Add(new Field("id", position.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("articul", position.Articul, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("name", position.Name, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("partnumber", position.PartNumber, Field.Store.YES, Field.Index.ANALYZED));
            if (position.Category != null)
            {
                document.Add(new Field("category", position.Category.Name, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }
            document.Add(new Field("price", SetPrice(position), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("distributor", position.Distributor.Name, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return document;
        }
        private string SetPrice(PositionCatalog position)
        {
            string resultPrice;
            if (position != null)
            {

                if (position.Currency != null && position.Currency.СultureInfo != "" && position.Currency.СultureInfo != null)
                {
                    IFormatProvider formatProvider = new System.Globalization.CultureInfo(position.Currency.СultureInfo);
                    resultPrice = position.Price.ToString("C", formatProvider);
                }
                else
                {
                    resultPrice = String.Format("{0:##,###.00}", position.Price);
                }
            }
            else
            {
                resultPrice = null;
            }

            return resultPrice;
        }
    }
}