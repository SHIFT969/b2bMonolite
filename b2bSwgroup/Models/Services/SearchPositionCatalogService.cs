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
using Lucene.Net.Util;
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
        public static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "IndexLucene");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }
        private static void _addToLuceneIndex(PositionCatalogIndexView position, IndexWriter writer)
        {
            //удалить старую запись индекса
            var searchQuery = new TermQuery(new Term("Id",position.Id.ToString()));
            writer.DeleteDocuments(searchQuery);
            //добавить новую запись индекса
            var doc = new Document();

            //Добавление поля lucene сопоставленые с полями db
            doc.Add(new Field("Id",position.Id.ToString(),Field.Store.YES,Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name",position.Name,Field.Store.YES,Field.Index.ANALYZED));
            doc.Add(new Field("Articul", position.Articul, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("PartNumber", position.PartNumber, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Category", position.Category, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Price", position.Price, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Distributor", position.Distributor, Field.Store.YES, Field.Index.ANALYZED));
            //добавление записи к индексу
            writer.AddDocument(doc);
        }

        public static void AddUpdateLuceneIndex(IEnumerable<PositionCatalogIndexView> positionsCatalog)
        {
            //Иницилизация Lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach(var position in positionsCatalog)
                {
                    _addToLuceneIndex(position, writer);
                }
                analyzer.Close();
                writer.Dispose();
            }
        }
        public static void AddUpdateLuceneIndex(PositionCatalogIndexView position)
        {
            AddUpdateLuceneIndex(new List<PositionCatalogIndexView> { position });
        }

        public static void ClearLuceneIndexRecord(int record_id)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory,analyzer,IndexWriter.MaxFieldLength.UNLIMITED))
            {
                //удаление записи из индекса
                var searchQuery = new TermQuery(new Term("Id",record_id.ToString()));
                writer.DeleteDocuments(searchQuery);

                analyzer.Close();
                writer.Dispose();
            }
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();

                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static PositionCatalogIndexView _mapLuceneDocumentToData(Document doc)
        {
            return new PositionCatalogIndexView
            {
                Id = Convert.ToInt32(doc.Get("Id")),
                Name = doc.Get("Name"),
                Articul = doc.Get("Articul"),
                Category = doc.Get("Category"),
                Distributor = doc.Get("Distributor"),
                PartNumber = doc.Get("PartNumber"),
                Price = doc.Get("Price"),
                Quantity = doc.Get("Quantity")  
            };
        }
        private static IEnumerable<PositionCatalogIndexView> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<PositionCatalogIndexView> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch(ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        

        private static IEnumerable<PositionCatalogIndexView> _search(string searchQuery, string searchField="")
        {
            //Валидация
            if(string.IsNullOrEmpty(searchQuery.Replace("*","").Replace("?","")))
            {
                return new List<PositionCatalogIndexView>();
            }

            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                if(!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                else
                {
                    var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new[] { "Name" },analyzer);
                    var query = parseQuery(searchQuery,parser);
                    var hits = searcher.Search(query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        public static IEnumerable<PositionCatalogIndexView> Search(string input, string fieldName = "")
        {
            if(string.IsNullOrEmpty(input))
            {
                return new List<PositionCatalogIndexView>();
            }
            var terms = input.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ",terms);

            return _search(input, fieldName);
        }

        public static IEnumerable<PositionCatalogIndexView> SearchDefault(string input, string fieldName="")
        {
            return string.IsNullOrEmpty(input) ? new List<PositionCatalogIndexView>() : _search(input,fieldName);
        }
        public static IEnumerable<PositionCatalogIndexView> GetAllIndexRecords()
        {
            if(!System.IO.Directory.EnumerateFiles(_luceneDir).Any())
            {
                return new List<PositionCatalogIndexView>();
            }
            var searcher = new IndexSearcher(_directory,false);
            var reader = IndexReader.Open(_directory,false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while(term.Next())
            {
                docs.Add(searcher.Doc(term.Doc));
            }
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);               
        }
    }
}