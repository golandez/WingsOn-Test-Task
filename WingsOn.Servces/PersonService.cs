using WingsOn.Dal;
using WingsOn.Domain;

namespace WingsOn.Servces
{
	public class PersonService : IPersonService
	{
		private IRepository<Person> _persons;

		public PersonService(IRepository<Person> personRepository)
		{
			_persons = personRepository;
		}

		public Person Get(int id)
		{
			return _persons.Get(id);
		}

		public void Save(Person element)
		{
			_persons.Save(element);
		}
	}
}
