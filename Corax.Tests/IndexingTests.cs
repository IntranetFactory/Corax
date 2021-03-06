﻿using System;
using System.Linq;
using Corax.Indexing;
using Corax.Queries;
using Voron;
using Xunit;

namespace Corax.Tests
{
    public class IndexingTests
    {
        [Fact]
        public void CanCreateAndDispose()
        {
            using (new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {

            }
        }

        [Fact]
        public void CanCreateAndDisposeIndexer()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (fti.CreateIndexer())
                {

                }
            }
        }

        [Fact]
        public void CanIndexEmptyDocument()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fti.CreateIndexer())
                {
                    indexer.NewIndexEntry();
                    indexer.Flush();
                }
            }
        }

        [Fact]
        public void CanIndexSingleValue()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fti.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.Flush();
                }
            }
        }

        [Fact]
        public void WillFilterStopWords()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fti.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren and Ayende");

                    indexer.Flush();
                }

                using (var searcher = fti.CreateSearcher())
                {
                    Assert.Empty(searcher.Query(new TermQuery("Name", "and")));
                }
            }
        }

        [Fact]
        public void CanQueryUsingSingleTerm()
        {
            using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.Index("Email", "ayende@ayende.com");

                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Arava Eini");

                    indexer.Index("Email", "arava@houseof.dog");

                    indexer.Flush();
                }

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }
            }
        }

        //[Fact]
        //public void CanQueryAndSort()
        //{
        //    using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
        //    {
        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {
        //            indexer.NewIndexEntry();

        //            indexer.Index("Name", "Oren Eini");

        //            indexer.Index("Email", "b@ayende.com");

        //            indexer.NewIndexEntry();

        //            indexer.Index("Name", "Arava Eini");

        //            indexer.Index("Email", "a@houseof.dog");

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            var results = searcher.QueryTop(new TermQuery("Name", "eini"), 5, sortBy: new Sorter("Email"));

        //            Assert.Equal(2, results.Results[0].DocumentId);
        //            Assert.Equal(1, results.Results[1].DocumentId);
        //        }
        //    }
        //}

        //[Fact]
        //public void CanQueryAndSortByTwoFields()
        //{
        //    using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
        //    {
        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {

        //            indexer.NewIndexEntry();
        //            indexer.Index("QueryFor", "yes");
        //            indexer.Index("FirstName", "David");
        //            indexer.Index("LastName", "Boike");

        //            indexer.NewIndexEntry();
        //            indexer.Index("QueryFor", "yes");
        //            indexer.Index("FirstName", "Natalie");
        //            indexer.Index("LastName", "Boike");

        //            indexer.NewIndexEntry();
        //            indexer.Index("QueryFor", "NO");
        //            indexer.Index("FirstName", "NO");
        //            indexer.Index("LastName", "NO");

        //            indexer.NewIndexEntry();
        //            indexer.Index("QueryFor", "yes");
        //            indexer.Index("FirstName", "Oren");
        //            indexer.Index("LastName", "Eini");

        //            indexer.NewIndexEntry();
        //            indexer.Index("QueryFor", "yes");
        //            indexer.Index("FirstName", "Arava");
        //            indexer.Index("LastName", "Eini");

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            var results = searcher.QueryTop(new TermQuery("QueryFor", "yes"), 5, sortBy: new Sorter(new SortByTerm("LastName"), new SortByTerm("FirstName")));

        //            Assert.Equal(4, results.Results.Length);
        //            Console.WriteLine("{0}, {1}, {2}, {3}", results.Results[0].DocumentId, results.Results[1].DocumentId,
        //                results.Results[2].DocumentId, results.Results[3].DocumentId);
        //            Assert.Equal(1, results.Results[0].DocumentId);
        //            Assert.Equal(2, results.Results[1].DocumentId);
        //            Assert.Equal(5, results.Results[2].DocumentId);
        //            Assert.Equal(4, results.Results[3].DocumentId);
        //        }
        //    }
        //}

        //[Fact]
        //public void CanDoPhraseQuery()
        //{
        //    using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
        //    {
        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {
        //            indexer.NewIndexEntry();
        //            indexer.Index("PhraseText", "Not a match.", options: FieldOptions.TermPositions);

        //            indexer.NewIndexEntry();
        //            indexer.Index("PhraseText", "RavenDB is a very cool database to work with.", options: FieldOptions.TermPositions);

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            var results = searcher.QueryTop(new PhraseQuery("PhraseText", "ravendb", "cool", "database")
        //            {
        //                Slop = 4
        //            }, 5);

        //            Assert.Equal(1, results.Results.Length);
        //            Assert.Equal(2, results.Results[0].DocumentId);
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            var results = searcher.QueryTop(new PhraseQuery("PhraseText", "database", "cool", "ravendb")
        //            {
        //                Slop = 4
        //            }, 5);

        //            Assert.Equal(0, results.Results.Length);
        //        }
        //    }
        //}

        //[Fact]
        //public void CanQueryWithInQuery()
        //{
        //    using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
        //    {
        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {

        //            indexer.NewIndexEntry();
        //            indexer.Index("Name", "David Boike");

        //            indexer.NewIndexEntry();
        //            indexer.Index("Name", "Oren Eini");

        //            indexer.NewIndexEntry();
        //            indexer.Index("Name", "Arava Eini");

        //            indexer.NewIndexEntry();
        //            indexer.Index("Name", "Sean Epping");

        //            indexer.NewIndexEntry();
        //            indexer.Index("Name", "Joe DeCock");

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            var results = searcher.QueryTop(new InQuery("Name", "joe", "epping", "boike"), 5);

        //            Assert.Equal(3, results.Results.Length);
        //        }
        //    }
        //}

        [Fact]
        public void CanQueryUsingBooleanQuery()
        {
            using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren");

                    indexer.Index("Email", "ayende@ayende.com");

                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren");

                    indexer.Index("Email", "oren@ayende.com");

                    indexer.Flush();
                }

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(1, searcher.Query(new BooleanQuery(QueryOperator.And,
                        new TermQuery("Name", "oren"),
                        new TermQuery("Email", "ayende@ayende.com"))).Count());

                    Assert.Equal(2, searcher.Query(new BooleanQuery(QueryOperator.Or,
                        new TermQuery("Name", "oren"),
                        new TermQuery("Email", "ayende@ayende.com"))).Count());
                }
            }
        }

        [Fact]
        public void CanDelete()
        {
            using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Ayende Rahien");

                    indexer.Flush();
                }

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }

                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.DeleteIndexEntry(1);

                    indexer.Flush();
                }

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }

                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.DeleteIndexEntry(2);

                    indexer.Flush();
                }

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }

            }
        }

        [Fact]
        public void CanDeleteAndCompact()
        {
            using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                fullTextIndex.Conventions.AutoCompact = false;
                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Ayende Rahien");

                    indexer.Flush();
                }

                using (var indexer = fullTextIndex.CreateIndexer())
                {
                    indexer.DeleteIndexEntry(1);

                    indexer.Flush();
                }


                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }
                Assert.Equal(2, fullTextIndex.NumberOfDocuments);
                Assert.Equal(1, fullTextIndex.NumberOfDeletes);
                fullTextIndex.RunCompaction();

                using (var searcher = fullTextIndex.CreateSearcher())
                {
                    Assert.Equal(0, searcher.Query(new TermQuery("Name", "oren")).Count());
                    Assert.Equal(1, searcher.Query(new TermQuery("Name", "rahien")).Count());
                }
                Assert.Equal(1, fullTextIndex.NumberOfDocuments);
                Assert.Equal(0, fullTextIndex.NumberOfDeletes);
                
            }
        }

        //[Fact]
        //public void CanQueryAndUpdate()
        //{
        //    using (var fullTextIndex = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
        //    {
        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {
        //            indexer.NewIndexEntry(); // doc 1

        //            Assert.Equal(1L, indexer.CurrentDocumentId);

        //            indexer.Index("Name", "Oren Eini");

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            Assert.Equal(1, searcher.Query(new TermQuery("Name", "oren")).Count());
        //        }

        //        using (var indexer = fullTextIndex.CreateIndexer())
        //        {
        //            indexer.UpdateIndexEntry(1); // doc 1

        //            Assert.Equal(1L, indexer.CurrentDocumentId);

        //            indexer.Index("Name", "Ayende Rahien");

        //            indexer.Flush();
        //        }

        //        using (var searcher = fullTextIndex.CreateSearcher())
        //        {
        //            Assert.Equal(0, searcher.Query(new TermQuery("Name", "oren")).Count());
        //            Assert.Equal(1, searcher.Query(new TermQuery("Name", "ayende")).Count());
        //        }
        //    }
        //}

        [Fact]
        public void CanQueryUsingMissingTerm()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fti.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.Flush();
                }

                using (var searcher = fti.CreateSearcher())
                {
                    Assert.Empty(searcher.Query(new TermQuery("Name", "Arava")));
                }
            }
        }

        [Fact]
        public void CanQueryUsingMissingField()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var indexer = fti.CreateIndexer())
                {
                    indexer.NewIndexEntry();

                    indexer.Index("Name", "Oren Eini");

                    indexer.Flush();
                }

                using (var searcher = fti.CreateSearcher())
                {
                    Assert.Empty(searcher.Query(new TermQuery("Foo", "Arava")));

                }
            }
        }

        [Fact]
        public void CanQueryOnEmptyindex()
        {
            using (var fti = new FullTextIndex(StorageEnvironmentOptions.CreateMemoryOnly(), new DefaultAnalyzer()))
            {
                using (var searcher = fti.CreateSearcher())
                {
                    Assert.Empty(searcher.Query(new TermQuery("Foo", "Arava")));
                }
            }
        }
    }
}
