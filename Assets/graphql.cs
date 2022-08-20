using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Defective.JSON;

public class graphql : MonoBehaviour
{
    public GraphApi lensReference;

    // Start is called before the first frame update
    void Start()
    {
        getProfile();
        explorePublications();
    }
    
    public async void getProfile(){
        GraphApi.Query query = lensReference.GetQueryByName("Profile", GraphApi.Query.Type.Query);
        query.SetArgs(new{request = new{profileId = "0x02"}});
        UnityWebRequest request = await lensReference.Post(query);
        //UnityWebRequest request = await lensReference.Post("getProfile", GraphApi.Query.Type.Query);
        string data = request.downloadHandler.text;
        Debug.Log(data);
    }

    public async void explorePublications() {
        //GraphApi.Query query = lensReference.GetQueryByName("ExplorePublications", GraphApi.Query.Type.Query);
        //query.SetArgs(new{request = new{sortCriteria = "LATEST_CREATED"}});
        UnityWebRequest request = await lensReference.Post(@"query ExplorePublications {
  explorePublications(request: {
    sortCriteria: TOP_COMMENTED,
    publicationTypes: [POST, COMMENT, MIRROR],
    limit: 10
  }) {
    items {
      __typename 
      ... on Post {
        ...PostFields
      }
      ... on Comment {
        ...CommentFields
      }
      ... on Mirror {
        ...MirrorFields
      }
    }
    pageInfo {
      prev
      next
      totalCount
    }
  }
}

fragment MediaFields on Media {
  url
  width
  height
  mimeType
}

fragment ProfileFields on Profile {
  id
  name
  bio
  attributes {
    displayType
    traitType
    key
    value
  }
  isFollowedByMe
  isFollowing(who: null)
  followNftAddress
  metadata
  isDefault
  handle
  picture {
    ... on NftImage {
      contractAddress
      tokenId
      uri
      verified
    }
    ... on MediaSet {
      original {
        ...MediaFields
      }
      small {
        ...MediaFields
      }
      medium {
        ...MediaFields
      }
    }
  }
  coverPicture {
    ... on NftImage {
      contractAddress
      tokenId
      uri
      verified
    }
    ... on MediaSet {
      original {
        ...MediaFields
      }
      small {
       ...MediaFields
      }
      medium {
        ...MediaFields
      }
    }
  }
  ownedBy
  dispatcher {
    address
  }
  stats {
    totalFollowers
    totalFollowing
    totalPosts
    totalComments
    totalMirrors
    totalPublications
    totalCollects
  }
  followModule {
    ... on FeeFollowModuleSettings {
      type
      amount {
        asset {
          name
          symbol
          decimals
          address
        }
        value
      }
      recipient
    }
    ... on ProfileFollowModuleSettings {
     type
    }
    ... on RevertFollowModuleSettings {
     type
    }
  }
}

fragment PublicationStatsFields on PublicationStats { 
  totalAmountOfMirrors
  totalAmountOfCollects
  totalAmountOfComments
}

fragment MetadataOutputFields on MetadataOutput {
  name
  description
  content
  media {
    original {
      ...MediaFields
    }
    small {
      ...MediaFields
    }
    medium {
      ...MediaFields
    }
  }
  attributes {
    displayType
    traitType
    value
  }
}

fragment Erc20Fields on Erc20 {
  name
  symbol
  decimals
  address
}

fragment CollectModuleFields on CollectModule {
  __typename
  ... on FreeCollectModuleSettings {
    type
  }
  ... on FeeCollectModuleSettings {
    type
    amount {
      asset {
        ...Erc20Fields
      }
      value
    }
    recipient
    referralFee
  }
  ... on LimitedFeeCollectModuleSettings {
    type
    collectLimit
    amount {
      asset {
        ...Erc20Fields
      }
      value
    }
    recipient
    referralFee
  }
  ... on LimitedTimedFeeCollectModuleSettings {
    type
    collectLimit
    amount {
      asset {
        ...Erc20Fields
      }
      value
    }
    recipient
    referralFee
    endTimestamp
  }
  ... on RevertCollectModuleSettings {
    type
  }
  ... on TimedFeeCollectModuleSettings {
    type
    amount {
      asset {
        ...Erc20Fields
      }
      value
    }
    recipient
    referralFee
    endTimestamp
  }
}

fragment PostFields on Post {
  id
  profile {
    ...ProfileFields
  }
  stats {
    ...PublicationStatsFields
  }
  metadata {
    ...MetadataOutputFields
  }
  createdAt
  collectModule {
    ...CollectModuleFields
  }
  referenceModule {
    ... on FollowOnlyReferenceModuleSettings {
      type
    }
  }
  appId
  hidden
  reaction(request: null)
  mirrors(by: null)
  hasCollectedByMe
}

fragment MirrorBaseFields on Mirror {
  id
  profile {
    ...ProfileFields
  }
  stats {
    ...PublicationStatsFields
  }
  metadata {
    ...MetadataOutputFields
  }
  createdAt
  collectModule {
    ...CollectModuleFields
  }
  referenceModule {
    ... on FollowOnlyReferenceModuleSettings {
      type
    }
  }
  appId
  hidden
  reaction(request: null)
  hasCollectedByMe
}

fragment MirrorFields on Mirror {
  ...MirrorBaseFields
  mirrorOf {
   ... on Post {
      ...PostFields          
   }
   ... on Comment {
      ...CommentFields          
   }
  }
}

fragment CommentBaseFields on Comment {
  id
  profile {
    ...ProfileFields
  }
  stats {
    ...PublicationStatsFields
  }
  metadata {
    ...MetadataOutputFields
  }
  createdAt
  collectModule {
    ...CollectModuleFields
  }
  referenceModule {
    ... on FollowOnlyReferenceModuleSettings {
      type
    }
  }
  appId
  hidden
  reaction(request: null)
  mirrors(by: null)
  hasCollectedByMe
}

fragment CommentFields on Comment {
  ...CommentBaseFields
  mainPost {
    ... on Post {
      ...PostFields
    }
    ... on Mirror {
      ...MirrorBaseFields
      mirrorOf {
        ... on Post {
           ...PostFields          
        }
        ... on Comment {
           ...CommentMirrorOfFields        
        }
      }
    }
  }
}

fragment CommentMirrorOfFields on Comment {
  ...CommentBaseFields
  mainPost {
    ... on Post {
      ...PostFields
    }
    ... on Mirror {
       ...MirrorBaseFields
    }
  }
}");
        string data = request.downloadHandler.text;
        JSONObject json = new JSONObject(data);
        JSONObject items = json.GetField("data").GetField("explorePublications").GetField("items");
        for (int i = 0; i < items.list.Count; i++)
        {
            JSONObject item = items.list[i];
            JSONObject id = item.GetField("id");
            JSONObject pictureUrl;
            try {
                pictureUrl = item.GetField("metadata").GetField("media")[0].GetField("original").GetField("url");
            } catch (System.Exception) {
                //Debug.Log("No picture");
                continue;
            }
            Debug.Log(id.stringValue);
            Debug.Log(pictureUrl);
            Vector3 newPosition = new Vector3(i * 1.2f, 0, 0);
            Vector3 newRotation = new Vector3(-90, 0, 0);
            GameObject picture = (GameObject)Instantiate(Resources.Load("Thumbnail"), newPosition, Quaternion.Euler(newRotation));
            picture.GetComponent<setTexture>().thumbnailUrl = pictureUrl.stringValue;


        }
        Debug.Log(data);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
