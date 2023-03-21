////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.jpa.sample.demo.dtos.securite;

import java.io.Serializable;

import jakarta.annotation.Generated;

import topmodel.jpa.sample.demo.entities.securite.Secteur;
import topmodel.jpa.sample.demo.entities.securite.SecuriteMappers;

/**
 * Objet métier non persisté représentant Profil.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public class SecteurDto implements Serializable {
	/** Serial ID */
	private static final long serialVersionUID = 1L;

	/**
	 * Id technique.
	 * Alias of {@link topmodel.jpa.sample.demo.entities.securite.Secteur#getId() Secteur#getId()} 
	 */
	private Long id;

	/**
	 * No arg constructor.
	 */
	public SecteurDto() {
	}

	/**
	 * Copy constructor.
	 * @param secteurDto to copy
	 */
	public SecteurDto(SecteurDto secteurDto) {
		if(secteurDto == null) {
			return;
		}

		this.id = secteurDto.getId();
	}

	/**
	 * All arg constructor.
	 * @param id Id technique
	 */
	public SecteurDto(Long id) {
		this.id = id;
	}

	/**
	 * Crée une nouvelle instance de 'SecteurDto'.
	 * @param secteur Instance de 'Secteur'.
	 *
	 * @return Une nouvelle instance de 'SecteurDto'.
	 */
	public SecteurDto(Secteur secteur) {
		SecuriteMappers.createSecteurDto(secteur, this);
	}

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.jpa.sample.demo.dtos.securite.SecteurDto#id id}.
	 */
	public Long getId() {
		return this.id;
	}

	/**
	 * Set the value of {@link topmodel.jpa.sample.demo.dtos.securite.SecteurDto#id id}.
	 * @param id value to set
	 */
	public void setId(Long id) {
		this.id = id;
	}

	/**
	 * Mappe 'SecteurDto' vers 'Secteur'.
	 * @param target Instance pré-existante de 'Secteur'. Une nouvelle instance sera créée si non spécifié.
	 *
	 * @return Une instance de 'Secteur'.
	 */
	public Secteur toSecteur(Secteur target) {
		return SecuriteMappers.toSecteur(this, target);
	}

	/**
	 * Enumération des champs de la classe {@link topmodel.jpa.sample.demo.dtos.securite.SecteurDto SecteurDto}.
	 */
	public enum Fields  {
        ID(Long.class);

		private Class<?> type;

		private Fields(Class<?> type) {
			this.type = type;
		}

		public Class<?> getType() {
			return this.type;
		}
	}
}
